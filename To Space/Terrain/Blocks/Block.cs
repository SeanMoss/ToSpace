using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using To_Space.Common;
using To_Space.Terrain;
using To_Space.Terrain.Materials;

namespace To_Space.Terrain.Blocks
{
	//A block is approximately 1 foot across
	public class Block
	{
		#region Block Properties
		//ID of the block
		private short _id;
		public short ID { get { return _id; } }

		//Name of the block
		private string _name;
		public string Name { get { return _name; } }

		//Texture index of this block
		private int _texture;
		public int TextureIndex { get { return _texture; } }

		#region Light / Textures
		//If it emits light
		public bool EmitsLight = false;

		//The intensity of the light emitted
		public float LightIntensity = 0.0f;

		//Color of the light emitted
		public Color LightColor = new Color(1f, 1f, 1f);

		//If the block should draw
		public bool ShouldDraw = true;

		//How much this block slows down entities walking on it
		public float Friction = .5f;
		#endregion

		//Material for stuff
		public Material Material = Material.Metal;

		//If the block requires per frame updates
		public bool SelfNotifyUpdate = false;

		//If the block needs to be updated when a neighbor changes
		public bool UpdateOnNeighborChange = false;
		#endregion

		public Block(short id, string name, int tex)
		{
			//Check id
			if (_idList.ContainsKey(id))
				throw new Exception(String.Format("The id {0} is already taken by another block.", id));

			_idList.Add(id, this);

			//Check name
			if (_nameList.ContainsKey(name))
				throw new Exception(String.Format("The name {0} is already taken by another block.", name));

			_nameList.Add(name, this);

			_id = id;
			_name = name;
			_texture = tex;

			_blockList.Add(this);

			TSDebug.WriteLine("BLOCK: \t\tRegistered new block-> " + this);
		}

		#region Frame Updates
		//Can override to tell how to block acts when a neighbor updates
		//x, y is the coordinates of the block changed
		//ALWAYS CALL base.UpdateNeighbors() IN ANY CHILD CLASSES
		public virtual void UpdateNeighbors(int x, int y, World world)
		{
			if (world.GetBlockAt(x, y) != null)
			{
				byte metaOld = world.GetMetadataAt(x, y);
				byte metaNew = this.GetMetadataForSurroundings(world, x, y);

				bool l = world.GetBlockAt(x - 1, y).UpdateOnNeighborChange;
				bool r = world.GetBlockAt(x + 1, y).UpdateOnNeighborChange;
				bool u = world.GetBlockAt(x, y - 1).UpdateOnNeighborChange;
				bool d = world.GetBlockAt(x, y + 1).UpdateOnNeighborChange;

				world.SetMetadataAt(x, y, metaNew, false);

				if (metaOld != metaNew)
				{
					if (l)
					{
						world.NotifyBlockOfNeighborChange(x - 1, y);
					}
					if (r)
					{
						world.NotifyBlockOfNeighborChange(x + 1, y);
					}
					if (u)
					{
						world.NotifyBlockOfNeighborChange(x, y - 1);
					}
					if (d)
					{
						world.NotifyBlockOfNeighborChange(x, y + 1);
					} 
				}
			}
		}

		//Called when the block is placed, used for special cases
		public virtual void OnBlockPlaced(int x, int y, World w)
		{

		}

		//Called when the block is removed
		public virtual void OnBlockRemoved(int x, int y, World w)
		{
			
		}
		#endregion

		#region Accessors
		//Gets the specific bounding box for this block type
		public virtual BoundingPolygon GetBoundingBoxBasedOnMetadata(byte meta)
		{
			return new BoundingPolygon
			(
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(0, 1),
				new Vector2(1, 1)
			);
		}
		//Returns the proper metadata based on the surrounding blocks
		public virtual byte GetMetadataForSurroundings(World w, int x, int y)
		{
			return 0;
		}
		#region Texture Control
		//Gets the index based on the metadata
		public virtual int GetIndexBasedOnMetadata(byte meta)
		{
			return _texture;
		}
		//Returns the path to the texture file
		public virtual string GetTexturePath()
		{
			return @"Textures\Blocks\";
		}
		//Returns the name of the texture file
		public virtual string GetTextureFile()
		{
			return @"ShipBuildBlocks";
		}
		//Returns the name of the block
		public override string ToString()
		{
			return this._name + ":" + this._id;
		}
		#endregion
		#endregion

		#region Static Members
		//List of block names
		private static Dictionary<string, Block> _nameList = new Dictionary<string, Block>();
		//List of block ids
		private static Dictionary<short, Block> _idList = new Dictionary<short, Block>();
		//General list of all added blocks
		private static List<Block> _blockList = new List<Block>();
		public static List<Block> BlockList { get { return _blockList; } }

		//Typecasting an int to a block
		public static implicit operator Block(short s)
		{
			if (!_idList.ContainsKey(s))
				return Block.Vacuum;

			return _idList[s];
		}
		public static implicit operator short(Block b)
		{
			return b.ID;
		}

		//size of one block in pixels
		public const int BLOCK_DIM = 32;
		#endregion

		#region Static Block List
		static Block()
		{
			Vacuum = new Block(0, "Vacuum", -1)
			{
				ShouldDraw = false,
				Material = Material.Gas,
				Friction = 0f
			};
			Air = new Block(1, "Air", -1)
			{
				ShouldDraw = false,
				Material = Material.Gas,
				Friction = 0f
			};
			SquareHull = new HullBlock(50, "Hull Square", 0);
			CarpetFloor = new CarpetFloorBlock(51, "Carpet", 16);
		}

		//Natural blocks
		public static readonly Block Vacuum;
		public static readonly Block Air;

		//Construction Blocks
		public static readonly Block SquareHull;
		public static readonly Block CarpetFloor;
		#endregion
	}
}