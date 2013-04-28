using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using To_Space.Common;
using To_Space.Terrain;
using To_Space.Terrain.Blocks;

namespace To_Space.Terrain
{
	public class RegionManager
	{
		//The region being managed
		private Region _region;
		public Region Region { get { return _region; } }

		public RegionManager(Region region)
		{
			_region = region;
		}

		public void Load()
		{
			Thread loadThread = new Thread(new ThreadStart(doLoad));
			loadThread.Name = "Region Load Thread";
			loadThread.Priority = ThreadPriority.Normal;
			_region.World.Game.ThreadManager.AddThread(loadThread);
			loadThread.Start();

			loadThread.Join();
			//_region.VerifyMetadata();
		}

		//will null reference things if unload is true
		public void Save()
		{
			Thread saveThread = new Thread(new ThreadStart(doSave));
			saveThread.Name = "Region Save Thread";
			saveThread.Priority = ThreadPriority.Normal;
			_region.World.Game.ThreadManager.AddThread(saveThread);
			saveThread.Start();

			saveThread.Join();
			//_region.VerifyMetadata();
		}

		public void Clear()
		{
			
		}

		private void doSave()
		{
			try
			{
				TSDebug.WriteLine("REGION: Starting to save region at " + _region.Position + " to disk.");

				CheckFilePath();

				//Flush the existing data
				FileStream clearStream = File.Open(this.GetSavePath(), FileMode.Create);
				clearStream.SetLength(0);
				clearStream.Flush();
				clearStream.Close();

				//Write the new data
				FileStream stream = File.Open(this.GetSavePath(), FileMode.OpenOrCreate);
				BinaryWriter writer = new BinaryWriter(stream);

				//Save the blocks
				for (int x = 0; x < Region.REGION_DIM; x++)
				{
					for (int y = 0; y < Region.REGION_DIM; y++)
					{
						writer.Write(_region.GetBlockAt(x, y).ID);
					}
				}

				//Save the lighting
				for (int x = 0; x < Region.REGION_DIM; x++)
				{
					for (int y = 0; y < Region.REGION_DIM; y++)
					{
						writer.Write(_region.GetRedLightAt(x, y));
						writer.Write(_region.GetGreenLightAt(x, y));
						writer.Write(_region.GetBlueLightAt(x, y));
					}
				}

				//Save the metadata
				for (int x = 0; x < Region.REGION_DIM; x++)
				{
					for(int y = 0; y < Region.REGION_DIM; y++)
					{
						writer.Write(_region.GetMetadataAt(x, y));
					}
				}

				//Save the backgrounds
				for (int x = 0; x < Region.REGION_DIM; x++)
				{
					for (int y = 0; y < Region.REGION_DIM; y++)
					{
						writer.Write(_region.GetBackgroundAt(x, y));
					}
				}

				writer.Flush();
				writer.Close();
				stream.Close();

				TSDebug.WriteLine("REGION: Saved region at " + _region.Position + " to disk.");
			}
			catch (IOException)
			{
				TSDebug.WriteLine("REGION: Could not save region at: " + _region.Position);
			}
			finally
			{
				_region.World.Game.ThreadManager.RemoveThread(Thread.CurrentThread);
			}
		}

		private void doLoad()
		{
			try
			{
				TSDebug.WriteLine("REGION: Starting to load region at " + _region.Position + " from disk.");

				if (CheckFilePath())
				{
					FileStream stream = File.OpenRead(this.GetSavePath());
					BinaryReader reader = new BinaryReader(stream);

					_region.InitializeRegion();
					for (int x = 0; x < Region.REGION_DIM; x++)
					{
						for (int y = 0; y < Region.REGION_DIM; y++)
						{
							_region.SetBlockAt(x, y, reader.ReadInt16(), false);
						}
					}

					_region.Lighting.Reinitialize();
					for (int x = 0; x < Region.REGION_DIM; x++)
					{
						for (int y = 0; y < Region.REGION_DIM; y++)
						{
							_region.SetRedLightAt(x, y, reader.ReadByte());
							_region.SetGreenLightAt(x, y, reader.ReadByte());
							_region.SetBlueLightAt(x, y, reader.ReadByte());
						}
					}

					//Load metadata
					for (int x = 0; x < Region.REGION_DIM; x++)
					{
						for (int y = 0; y < Region.REGION_DIM; y++)
						{
							_region.SetMetadataAt(x, y, reader.ReadByte());
						}
					}

					//Load backgrounds
					for (int x = 0; x < Region.REGION_DIM; x++)
					{
						for (int y = 0; y < Region.REGION_DIM; y++)
						{
							_region.SetBackgroundAt(x, y, reader.ReadInt16());
						}
					}

					reader.Close();
					stream.Close();

					TSDebug.WriteLine("REGION: Finished loading the region at " + _region.Position + " from the disk.");
				}
				else
				{
					TSDebug.WriteLine("No region to load at " + _region.Position);
				}
			}
			catch (IOException)
			{
				TSDebug.WriteLine("Could not load region at " + _region.Position + " from disk. Clearing region.");
			}
			finally
			{
				_region.World.Game.ThreadManager.RemoveThread(Thread.CurrentThread);
			}
		}

		private bool CheckFilePath()
		{
			try
			{
				if (!File.Exists(this.GetSavePath()))
				{
					File.Create(this.GetSavePath());
					TSDebug.WriteLine("REGION: Creating region save file for region at " + _region.Position);
					return false;
				}

				return true;
			}
			catch (IOException)
			{
				TSDebug.WriteLine("REGION: Cannot create the region file for world " + _region.World + " and region " + _region.Position);
				return false;
			}
		}

		public string GetSavePath()
		{
			return _region.World.GetSavePath() + @"Regions\" + String.Format("{0}-{1}.tsr", _region.Position.X, _region.Position.Y);
		}

		public static string GetSavePath(Vector2i vec, World world)
		{
			return world.GetSavePath() + @"Regions\" + String.Format("{0}-{1}.tsr", vec.X, vec.Y);
		}
	}
}