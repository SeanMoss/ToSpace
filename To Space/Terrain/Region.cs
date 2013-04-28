using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using To_Space.Terrain.Backgrounds;
using To_Space.Terrain.Blocks;
using To_Space.Common;

namespace To_Space.Terrain
{
	public class Region
	{
		public static readonly int REGION_DIM = 32;

		//If this region is loaded
		private bool _loaded = false;
		public bool Loaded { get { return _loaded; } }

		//The array of blocks for this region
		private short[,] _blocks;
		public short[,] Blocks { get { return _blocks; } }

		//The region manager for this region
		private RegionManager _regionManager;
		public RegionManager RManager { get { return _regionManager; } }

		//The lighting manager for this region
		private LightingManager _lighting;
		public LightingManager Lighting { get { return _lighting; } }

		//Position of this region
		private Vector2i _position;
		public Vector2i Position { get { return _position; } }

		//Block position of this region
		public Vector2i AbsolutePosition { get { return new Vector2i(_position.X * REGION_DIM, _position.Y * REGION_DIM); } }

		//The world for this region
		private World _world;
		public World World { get { return _world; } }

		//If the region is not synced with the disk
		private bool _dirty = true;
		public bool Dirty { get { return _dirty; } }

		//The metadata for the region
		private byte[,] _metadata;
		public byte[,] Metadata { get { return _metadata; } }

		//The backgrounds
		private short[,] _background;
		public short[,] Backgrounds { get { return _background; } }

		private SpriteFont _font;

		public Region(World world, Vector2i pos)
		{
			_world = world;
			_position = pos;

			_regionManager = new RegionManager(this);
			_lighting = new LightingManager(this);

			_font = _world.Game.Content.Load<SpriteFont>(@"Fonts\console");

			InitializeRegion();

			TryLoad();
		}

		public void InitializeRegion()
		{
			_blocks = new short[REGION_DIM, REGION_DIM];
			_metadata = new byte[REGION_DIM, REGION_DIM];
			_background = new short[REGION_DIM, REGION_DIM];

			for (int x = 0; x < REGION_DIM; x++)
			{
				for (int y = 0; y < REGION_DIM; y++)
				{
					_blocks[x, y] = Block.Vacuum;
					_metadata[x, y] = 0;
					_background[x, y] = Background.Empty;
				}
			}

			_dirty = true;
		}

		public void VerifyMetadata()
		{
			for (int x = 0; x < REGION_DIM; x++)
			{
				for (int y = 0; y < REGION_DIM; y++)
				{
					this.SetMetadataAt(x, y, this.GetBlockAt(x, y).GetMetadataForSurroundings(_world, x, y));
				}
			}

			for (int y = 0; y < REGION_DIM; y++)
			{
				for (int x = 0; x < REGION_DIM; x++)
				{
					this.SetMetadataAt(x, y, this.GetBlockAt(x, y).GetMetadataForSurroundings(_world, x, y));
				}
			}
		}

		#region Frame Methods
		//Updates this Region
		public void Update(GameTime gameTime)
		{

		}

		//Draws the region
		public void Draw(SpriteBatch sBatch)
		{
			int width = Block.BLOCK_DIM;
			int startx = (this._position.X * REGION_DIM * width) - (int)_world.Camera.Offset.X;
			int starty = (this._position.Y * REGION_DIM * width) - (int)_world.Camera.Offset.Y;

			for (int x = 0; x < REGION_DIM; x++)
			{
				for (int y = 0; y < REGION_DIM; y++)
				{
					#region Draw Backgrounds
					Background g = _background[x, y];
					Block b = _blocks[x, y];

					if (g.ShouldDraw)
					{
						Texture2D tex = TextureManager.GetTexture(g.GetTextureFile());
						Rectangle source = TextureManager.GetSourceForIndex(g.GetIndexForSurroundings(_world, this.AbsolutePosition.X + x, this.AbsolutePosition.Y + y), tex);
						source.Width -= 1; //Adjust for broken textures
						Rectangle draw = new Rectangle(startx + x * width, starty + y * width, width, width);

						sBatch.Draw(tex, draw, source, Color.White);
					}
					#endregion

					#region Draw Blocks
					if (b.ShouldDraw)
					{
						Texture2D tex = TextureManager.GetTexture(b.GetTextureFile());
						Rectangle source = TextureManager.GetSourceForIndex(b.GetIndexBasedOnMetadata(this.GetMetadataAt(x, y)), tex);
						source.Width -= 1; //Adjust for broken textures
						Rectangle draw = new Rectangle(startx + x * width, starty + y * width, width, width);

						if (b is BuildBlock)
						{
							sBatch.Draw(tex, draw, source, Color.White);
							//sBatch.DrawString(_font, this.GetMetadataAt(x, y).ToString(), new Vector2(startx + x * width, starty + y * width), Color.Red);
						}
						else
						{
							sBatch.Draw(tex, draw, source, Color.White);
							//sBatch.DrawString(_font, this.GetMetadataAt(x, y).ToString(), new Vector2(startx + x * width, starty + y * width), Color.Red);
						}
					}
					#endregion
				}
			}
		}
		#endregion

		#region Accessors
		//Gets the block
		public Block GetBlockAt(int x, int y)
		{
			if (IsInRange(x, y))
			{
				return _blocks[x, y]; 
			}

			return null;
		}

		public void SetBlockAt(int x, int y, Block b)
		{
			if (IsInRange(x, y))
			{
				Block old = _blocks[x, y];
				_blocks[x, y] = b;
				old.OnBlockRemoved(this.AbsolutePosition.X + x, this.AbsolutePosition.Y + y, _world);
				b.OnBlockPlaced(this.AbsolutePosition.X + x, this.AbsolutePosition.Y + y, _world);
				_dirty = true;
				_world.NotifyBlockOfNeighborChange(this.AbsolutePosition.X + x, this.AbsolutePosition.Y + y);
			}
		}

		public void SetBlockAt(int x, int y, Block b, bool update)
		{
			if (IsInRange(x, y))
			{
				Block old = _blocks[x, y];
				_blocks[x, y] = b;
				_dirty = true;
				if (update)
				{
					old.OnBlockRemoved(x, y, _world);
					b.OnBlockPlaced(x, y, _world);
					_world.NotifyBlockOfNeighborChange(this.AbsolutePosition.X + x, this.AbsolutePosition.Y + y); 
				}
			}
		}

		public byte GetMetadataAt(int x, int y)
		{
			if (IsInRange(x, y))
			{
				return _metadata[x, y]; 
			}

			return 0;
		}

		public void SetMetadataAt(int x, int y, byte b)
		{
			if (IsInRange(x, y))
			{
				_metadata[x, y] = b;
				_dirty = true;
				_world.NotifyBlockOfNeighborChange(this.AbsolutePosition.X + x, this.AbsolutePosition.Y + y);
			}
		}

		public void SetMetadataAt(int x, int y, byte b, bool update)
		{
			if (IsInRange(x, y))
			{
				_metadata[x, y] = b;
				_dirty = true;

				if (update)
				{
					_world.NotifyBlockOfNeighborChange(this.AbsolutePosition.X + x, this.AbsolutePosition.Y + y); 
				}
			}
		}

		public Background GetBackgroundAt(int x, int y)
		{
			if (IsInRange(x, y))
			{
				return _background[x, y];
			}

			return 0;
		}

		public void SetBackgroundAt(int x, int y, Background b)
		{
			if (IsInRange(x, y))
			{
				_background[x, y] = b;
				_dirty = true;
			}
		}

		#region Lighting
		public void SetRedLightAt(int x, int y, byte b)
		{
			if (IsInRange(x, y))
			{
				_lighting.SetRedLightAt(x, y, b);
			}
		}

		public void SetGreenLightAt(int x, int y, byte b)
		{
			if (IsInRange(x, y))
			{
				_lighting.SetGreenLightAt(x, y, b);
			}
		}

		public void SetBlueLightAt(int x, int y, byte b)
		{
			if (IsInRange(x, y))
			{
				_lighting.SetBlueLightAt(x, y, b);
			}
		}

		public Color GetLightAt(int x, int y)
		{
			if (IsInRange(x, y))
			{
				return _lighting.GetLightAt(x, y);
			}

			return new Color(1, 1, 1, 1);
		}

		public byte GetRedLightAt(int x, int y)
		{
			if (IsInRange(x, y))
			{
				return _lighting.GetRedLightAt(x, y);
			}

			return 0;
		}

		public byte GetGreenLightAt(int x, int y)
		{
			if (IsInRange(x, y))
			{
				return _lighting.GetGreenLightAt(x, y);
			}

			return 0;
		}

		public byte GetBlueLightAt(int x, int y)
		{
			if (IsInRange(x, y))
			{
				return _lighting.GetBlueLightAt(x, y);
			}

			return 0;
		}
		#endregion
		#endregion

		#region Utilities
		public bool IsInRange(int x, int y)
		{
			return !(x < 0 || y < 0 || x >= REGION_DIM || y >= REGION_DIM);
		}

		public void OffsetRectangle(ref BoundingPolygon b, int x, int y)
		{
			for(int i = 0; i < b.Points.Count; i++)
			{
				Vector2 p = new Vector2(b.Points[i].X, b.Points[i].Y);
				p.X += this.AbsolutePosition.X + x;
				p.Y += this.AbsolutePosition.Y + y;
				b.Points[i] = p;
			}
		}

		public void SaveRegion(bool unload)
		{
			if (_dirty)
			{
				_regionManager.Save();
				_dirty = false;
			}

			if (unload)
			{
				_lighting.Unload();
				_blocks = null;
				_metadata = null;

				_loaded = false;

				_dirty = false;
			}
		}

		public void ForceSave()
		{
			_regionManager.Save();
		}

		public void TryLoad()
		{
			if (File.Exists(RegionManager.GetSavePath(this._position, this._world)))
			{
				LoadRegion();
			}
		}

		public void LoadRegion()
		{
			_regionManager.Load();

			_loaded = true;
			_dirty = true;
		}
		#endregion

		#region Neighbors
		public Region Left { get { return _world.GetRegion(this._position.X - 1, this._position.Y); } }
		public Region Right { get { return _world.GetRegion(this._position.X + 1, this._position.Y); } }
		public Region Up { get { return _world.GetRegion(this._position.X, this._position.Y - 1); } }
		public Region Down { get { return _world.GetRegion(this._position.X, this._position.Y + 1); } }
		#endregion
	}
}