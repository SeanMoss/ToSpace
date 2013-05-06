using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using To_Space.Common;
using To_Space.Items;
using To_Space.Terrain;
using To_Space.Terrain.Blocks;
using To_Space.Terrain.Backgrounds;
using To_Space.Entities;

namespace To_Space.Terrain
{
	public class World
	{
		//Game
		private ToSpaceGame _game;
		public ToSpaceGame Game { get { return _game; } }

		//Name of this world
		private string _name;
		public string Name { get { return _name; } }
		
		//Region list for the world
		private Region[,] _regions;
		public Region[,] Regions { get { return _regions; } }

		//Width of the map in regions
		private int _rWidth;
		public int RegionWidth { get { return _rWidth; } }

		//Height of the map in regions
		private int _rHeight;
		public int RegionHeight { get { return _rHeight; } }

		//Camera for this world
		private WorldCamera _camera;
		public WorldCamera Camera { get { return _camera; } }

		//Background for this world
		private ParallaxingBackground _background;
		public ParallaxingBackground Background { get { return _background; } }

		//The player that this computer is controlling
		private Player _player;
		public Player Player { get { return _player; } }

		//If this world is newly created
		private bool _new;

		//The list of places to update
		private List<Vector2i> _needsUpdate = new List<Vector2i>();
		
		//Temp debug font
		private SpriteFont _font;

		public World(int width, int height, string name, ToSpaceGame game)
		{
			_game = game;

			_rWidth = width;
			_rHeight = height;

			_camera = new WorldCamera(this);
			Texture2D player = _game.Content.Load<Texture2D>(@"Entites\Player");
			_player = new Player(new Vector2(72, 2000), player, this);

			_camera.SetFocusOn(_player);

			_font = this._game.Content.Load<SpriteFont>(@"Fonts\console");

			_name = name;

			//Create background
			_background = new ParallaxingBackground(this);
			_background.Textures[0] = Game.Content.Load<Texture2D>(@"Textures\Backgrounds\Starfield");
			_background.XScrollSpeeds[0] = .09f;
			_background.YScrollSpeeds[0] = .09f;
			_background.UpKey = Keys.Down;
			_background.DownKey = Keys.Up;
			_background.LeftKey = Keys.Right;
			_background.RightKey = Keys.Left;

			BlockPen.Init(this);
			//ColorCycler.Init(this);

			InitializeRegions();

			//Create save directory for this world
			if (!Directory.Exists(this.GetSavePath()) || !Directory.Exists(this.GetSavePath() + @"Regions\") ||
				!Directory.Exists(this.GetSavePath() + @"Players\") || !Directory.Exists(this.GetSavePath() + @"Pictures\"))
			{
				if (!Directory.Exists(this.GetSavePath()))
				{
					Directory.CreateDirectory(this.GetSavePath()); 
				}
				if (!Directory.Exists(this.GetSavePath() + @"Regions\"))
				{
					Directory.CreateDirectory(this.GetSavePath() + @"Regions\"); 
				}
				if (!Directory.Exists(this.GetSavePath() + @"Players\"))
				{
					Directory.CreateDirectory(this.GetSavePath() + @"Players\"); 
				}
				if (!Directory.Exists(this.GetSavePath() + @"Pictures\"))
				{
					Directory.CreateDirectory(this.GetSavePath() + @"Pictures\");
				}

				_new = true;

				TSDebug.WriteLine("WORLD: Created new save directory for world: " + name);

				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						if (!File.Exists(RegionManager.GetSavePath(new Vector2i(x, y), this)))
						{
							File.Create(RegionManager.GetSavePath(new Vector2i(x, y), this)); 
						}
					}
				}
			}
			else
			{
				_new = false;

				TSDebug.WriteLine("WORLD: Found save directory for world: " + name);
			}

			ForceWorldSave(false);
		}

		private void InitializeRegions()
		{
			_regions = new Region[_rWidth, _rHeight];

			for (int x = 0; x < _rWidth; x++)
			{
				for (int y = 0; y < _rHeight; y++)
				{
					_regions[x, y] = new Region(this, new Vector2i(x, y));
				}
			}
		}

		#region Frame Methods
		//Updates the enitre loaded world
		int saveTime = 0;
		public void Update(GameTime gameTime)
		{
			saveTime++;
			if (saveTime > 1800)
			{
				saveTime = 0;
				ForceWorldSave(true);
			}

			if (Game.InputManager.IsKeyPressed(Keys.T))
			{
				_player.Position = new Vector2(3000, 3000);
			}

			_camera.Update();

			BlockPen.Update();
			//ColorCycler.Update();

			//_player.Update();

			_player.Update();

			_background.Update();
		}

		//Draws the proper area in the loaded world
		public void Draw(SpriteBatch sBatch, GameTime gameTime)
		{
			_background.Draw(sBatch, Color.White);

			updateDrawList();

			sBatch.Begin();

			foreach (Vector2i vec in _toDraw)
			{
				_regions[vec.X, vec.Y].Draw(sBatch);
			}

			_player.Draw(sBatch);

			//sBatch.DrawString(_font, "\"To Space\" Demo Controls:", new Vector2(0, 0), Color.LightGreen);
			//sBatch.DrawString(_font, "Use A/D to move the character, Space to jump.", new Vector2(0, 20), Color.LightGreen);
			//sBatch.DrawString(_font, "Press R to go to start point. Press T to go to building portion.", new Vector2(0, 40), Color.LightGreen);
			//sBatch.DrawString(_font, "Press Q to draw backgrounds instead of blocks. Use mouse button to draw blocks.", new Vector2(0, 60), Color.LightGreen);
			//sBatch.DrawString(_font, "Press X to delete blocks.", new Vector2(0, 80), Color.LightGreen);
			//sBatch.DrawString(_font, "Press Escape to quit the game.", new Vector2(0, 100), Color.LightGreen);

			sBatch.DrawString(_font, "FPS: " + Math.Round((1 / gameTime.ElapsedGameTime.TotalSeconds), 3), new Vector2(0, 720), Color.Red);
			sBatch.DrawString(_font, "Player position: " + this._player.Position.X + " " + this._player.Position.Y, new Vector2(0, 740), Color.Red);

			sBatch.End();
		}

		private List<Vector2i> _toDraw = new List<Vector2i>();
		//Updates the list of regions to draw
		private void updateDrawList()
		{
			_toDraw.Clear();

			//Vector2i screen = new Vector2i(_game.GraphicsDevice.PresentationParameters.BackBufferWidth,
			//	_game.GraphicsDevice.PresentationParameters.BackBufferHeight);

			//Vector2i start = new Vector2i((int)-this._camera.Offset.X / _camera.Zoom, (int)-this._camera.Offset.Y / _camera.Zoom);
			//Vector2i end = new Vector2i((int)(-this._camera.Offset.X + screen.X) / _camera.Zoom, 
			//	(int)(-this._camera.Offset.Y + screen.Y) / _camera.Zoom);

			//start.X = start.X >= 0 ? start.X : 0;
			//start.Y = start.Y >= 0 ? start.Y : 0;
			//end.X = end.X < _rWidth ? end.X : _rWidth - 1;
			//end.Y = end.Y < _rHeight ? end.Y : _rHeight - 1;

			//for (int x = start.X; x <= end.X; x++)
			//{
			//	for (int y = start.Y; y <= end.Y; y++)
			//	{
			//		_toDraw.Add(new Vector2i(x, y));
			//	}
			//}

			for (int i = 0; i < _rWidth; i++)
			{
				for (int j = 0; j < _rHeight; j++)
				{
					_toDraw.Add(new Vector2i(i, j));
				}
			}
		}
		#endregion

		#region Block Accessors
		public Block GetBlockAt(int x, int y)
		{
			Region r = GetRegionAt(x, y);

			x %= Region.REGION_DIM;
			y %= Region.REGION_DIM;

			if (r != null)
			{
				return r.GetBlockAt(x, y); 
			}

			return Block.Vacuum;
		}

		public void SetBlockAt(int x, int y, Block b)
		{
			Region r = this.GetRegionAt(x, y);

			if (r != null)
			{
				r.SetBlockAt(x % Region.REGION_DIM, y % Region.REGION_DIM, b);
			}
		}

		public void SetBlockAt(int x, int y, Block b, byte meta)
		{
			Region r = this.GetRegionAt(x, y);

			if (r != null)
			{
				r.SetBlockAt(x % Region.REGION_DIM, y % Region.REGION_DIM, b);
				r.SetMetadataAt(x % Region.REGION_DIM, y % Region.REGION_DIM, meta);
			}
		}

		//Gets the region based on block coordinates
		public Region GetRegionAt(int x, int y)
		{
			if (this.BlockInWorld(x, y))
			{
				return _regions[x / Region.REGION_DIM, y / Region.REGION_DIM];
			}

			return null;
		}

		//Gets the region based on region coordinates
		public Region GetRegion(int x, int y)
		{
			if (this.RegionInWorld(x, y))
			{
				return _regions[x, y]; 
			}

			return null;
		}

		public byte GetMetadataAt(int x, int y)
		{
			Region r = GetRegionAt(x, y);

			if (r != null)
			{
				return r.GetMetadataAt(x % Region.REGION_DIM, y % Region.REGION_DIM);
			}
			else
				return 0;
		}

		public void SetMetadataAt(int x, int y, byte b)
		{
			Region r = GetRegionAt(x, y);

			if (r != null)
			{
				r.SetMetadataAt(x % Region.REGION_DIM, y % Region.REGION_DIM, b);
			}
		}

		public void SetMetadataAt(int x, int y, byte b, bool update)
		{
			Region r = GetRegionAt(x, y);

			if (r != null)
			{
				r.SetMetadataAt(x % Region.REGION_DIM, y % Region.REGION_DIM, b, update);
			}
		}

		public Background GetBackgroundAt(int x, int y)
		{
			Region r = GetRegionAt(x, y);

			if (r != null)
			{
				return r.GetBackgroundAt(x % Region.REGION_DIM, y % Region.REGION_DIM);
			}

			return 0;
		}

		public void SetBackgroundAt(int x, int y, Background b)
		{
			Region r = GetRegionAt(x, y);

			if (r != null)
			{
				r.SetBackgroundAt(x % Region.REGION_DIM, y % Region.REGION_DIM, b);
			}
		}
		#endregion

		//Marks that the block at x, y was changed, notifies neighbors if it has to
		public void NotifyBlockOfNeighborChange(int x, int y)
		{
			this.GetBlockAt(x, y).UpdateNeighbors(x, y, this);
		}

		public void ForceWorldSave(bool useDirty)
		{
			for (int x = 0; x < _rWidth; x++)
			{
				for (int y = 0; y < _rHeight; y++)
				{
					if (useDirty)
					{
						_regions[x, y].SaveRegion(false);
					}
					else
					{
						_regions[x, y].ForceSave();
					}
				}
			}
		}

		#region Utilities
		public bool BlockInWorld(int x, int y)
		{
			return !(x < 0 || y < 0 || x >= this._rWidth * Region.REGION_DIM || y >= this._rHeight * Region.REGION_DIM);
		}

		public bool RegionInWorld(int x, int y)
		{
			return !(x < 0 || y < 0 || x >= this._rWidth || y >= this._rHeight);
		}

		#region Solid Block Locators
		//Gets the nearest ground starting from the point (i, j)
		public Vector2 NearestGroundFrom(int x, int y)
		{
			int i = x;
			int j = y;

			int counter = 0;
			while (j < this._rHeight * 32 && counter < 15)
			{
				if (this.GetBlockAt(i, j).Material.Solid)
				{
					return new Vector2(i, j);
				}

				j++;
				counter++;
			}

			return new Vector2(-1, -1);
		}

		//Gets the nearest ceiling starting from the point (i, j)
		public Vector2 NearestCeilingFrom(int x, int y)
		{
			int i = x;
			int j = y;

			int counter = 0;
			while (j >= 0 && counter < 15)
			{
				if (this.GetBlockAt(i, j).Material.Solid)
				{
					return new Vector2(i, j);
				}

				j--;
				counter++;
			}

			return new Vector2(-1, -1);
		}

		//Gets the nearest wall to the right starting from the point (i, j)
		public Vector2 NearestRightWallFrom(int x, int y)
		{
			int i = x;
			int j = y;

			int counter = 0;
			while (i < this._rWidth * 32 && counter < 15)
			{
				if (this.GetBlockAt(i, j).Material.Solid)
				{
					return new Vector2(i, j);
				}

				i++;
				counter++;
			}

			return new Vector2(-1, -1);
		}

		//Gets the nearest wall to the left starting from the point (i, j)
		public Vector2 NearestLeftWallFrom(int x, int y)
		{
			int i = x - 1;
			int j = y;

			int counter = 0;
			while (i >= 0 && counter < 15)
			{
				if (this.GetBlockAt(i, j).Material.Solid)
				{
					return new Vector2(i, j);
				}

				i--;
				counter++;
			}

			return new Vector2(-1, -1);
		}

		//Gets the absolute distance to the nearest ground starting from the point (i, j)
		public float? DistanceToNearestGroundFrom(Vector2 pos)
		{
			Vector2 nearestGround = this.NearestGroundFrom((int)(pos.X / 32), (int)(pos.Y / 32));

			if (!nearestGround.Equals(new Vector2(-1, -1)))
			{
				float endPoint = nearestGround.Y * 32;

				return Math.Abs(endPoint - pos.Y); 
			}

			return null;
		}

		//Gets the absolute distance to the nearest ceiling starting from the point (i, j)
		public float? DistanceToNearestCeilingFrom(Vector2 pos)
		{
			Vector2 nearestCeiling = this.NearestCeilingFrom((int)(pos.X / 32), (int)(pos.Y / 32));

			if (!nearestCeiling.Equals(new Vector2(-1, -1)))
			{
				float endPoint = nearestCeiling.Y * 32;

				return Math.Abs(endPoint - pos.Y) - 32; 
			}

			return null;
		}

		//Gets the absolute distance to the nearest wall to the right starting from the point (i, j)
		public float? DistanceToNearestRightWallFrom(Vector2 pos)
		{
			Vector2 nearestWall = this.NearestRightWallFrom((int)(pos.X / 32), (int)(pos.Y / 32));

			if (!nearestWall.Equals(new Vector2(-1, -1)))
			{
				float endPoint = nearestWall.X * 32;

				return Math.Abs(endPoint - pos.X); 
			}

			return null;
		}

		//Gets the absolute distance to the nearest wall to the left starting from the point (i, j)
		public float? DistanceToNearestLeftWallFrom(Vector2 pos)
		{
			Vector2 nearestWall = this.NearestLeftWallFrom((int)(pos.X / 32), (int)(pos.Y / 32));

			if (!nearestWall.Equals(new Vector2(-1, -1)))
			{
				float endPoint = nearestWall.X * 32;

				return Math.Abs(endPoint - pos.X) - 32; 
			}

			return null;
		}

		#region Polygon Getting
		//Gets the Polygon at the nearest ground
		public BoundingPolygon GetBoundingAtNearestGroundFrom(int x, int y)
		{
			Vector2 next = this.NearestGroundFrom((int)(x / 32), (int)(y / 32));

			BoundingPolygon poly = this.GetBlockAt((int)next.X, (int)next.Y).GetBoundingBoxBasedOnMetadata(0);

			poly += next;

			return poly;
		}

		//Gets the Polygon at the nearest ceiling
		public BoundingPolygon GetBoundingAtNearestCeilingFrom(int x, int y)
		{
			Vector2 next = this.NearestCeilingFrom((int)(x / 32), (int)(y / 32));

			BoundingPolygon poly = this.GetBlockAt((int)next.X, (int)next.Y).GetBoundingBoxBasedOnMetadata(0);

			poly += next;

			return poly;
		}

		//Gets the Polygon at the nearest right wall
		public BoundingPolygon GetBoundingAtNearestRightWallFrom(int x, int y)
		{
			Vector2 next = this.NearestRightWallFrom((int)(x / 32), (int)(y / 32));

			BoundingPolygon poly = this.GetBlockAt((int)next.X, (int)next.Y).GetBoundingBoxBasedOnMetadata(0);

			poly += next;

			return poly;
		}

		//Gets the Polygon at the nearest left wall
		public BoundingPolygon GetBoundingAtNearestLeftWallFrom(int x, int y)
		{
			Vector2 next = this.NearestLeftWallFrom((int)(x / 32), (int)(y / 32));

			BoundingPolygon poly = this.GetBlockAt((int)next.X, (int)next.Y).GetBoundingBoxBasedOnMetadata(0);

			poly += next;

			return poly;
		}
		#endregion
		#endregion
		#endregion

		private SpriteBatch _spriteBatch;
		public bool SaveFullMapPicture(SpriteBatch batch)
		{
			//try
			//{
			this._spriteBatch = batch;
			doSave();
			return true;

			//	Thread t = new Thread(new ThreadStart(doSave));
			//	t.Name = "Map Picture Maker";
			//	t.Priority = ThreadPriority.BelowNormal;
			//	Game.ThreadManager.AddThread(t);
			//	t.Start();

			//	t.Join();

			//	return true;
			//}
			//catch (Exception)
			//{
			//	return false;
			//}
		}

		private void doSave()
		{
#if WINDOWS
			int texWidth = this._rWidth * Region.REGION_DIM * 16;
			int texHeight = this._rHeight * Region.REGION_DIM * 16;

			RenderTarget2D renderTarget = new RenderTarget2D(Game.GraphicsDevice, texWidth, texHeight);
			Game.GraphicsDevice.SetRenderTarget(renderTarget);
			Game.GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

			for (int blockX = 0; blockX < texWidth / 16; blockX++)
			{
				for (int blockY = 0; blockY < texHeight / 16; blockY++)
				{
					Block b = this.GetBlockAt(blockX, blockY);
					Texture2D toDraw = TextureManager.GetTextureAtIndex(b.GetIndexBasedOnMetadata(b.GetMetadataForSurroundings(this, blockX, blockY)), b.GetTextureFile());

					if (toDraw != null)
					{
						_spriteBatch.Draw(toDraw, new Rectangle(blockX * 16, blockY * 16, 16, 16), Color.White);
					}
				}
			}

			_spriteBatch.End();

			string fileName = "WorldRender" + DateTime.Now.ToString(@"MM\-dd\-yyyy-h\-mm-ss-tt");
			Stream stream = File.Open(this.GetSavePath() + @"Pictures\" + fileName, FileMode.OpenOrCreate);

			Texture2D picture = (Texture2D)renderTarget;
			picture.SaveAsPng(stream, picture.Width, picture.Height); 
#endif
		} 

		public string GetSavePath()
		{
			return GameSettings.GamePath + @"Saves\" + this._name + @"\";
		}
	}
}