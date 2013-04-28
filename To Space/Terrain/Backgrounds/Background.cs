using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace To_Space.Terrain.Backgrounds
{
	public class Background
	{
		#region Background Properties
		//Id of this background
		private short _id;
		public short ID { get { return _id; } }

		//Name of the background
		private string _name;
		public string Name { get { return _name; } }

		//The index of the texture
		private int _texture;
		public int TextureIndex { get { return _texture; } }

		//If the texture should draw
		public bool ShouldDraw = true;
		#endregion

		public Background(short id, string name, int tex)
		{
			//Check id
			if (_idList.ContainsKey(id))
				throw new Exception(String.Format("The id {0} is already taken by another background.", id));

			_idList.Add(id, this);

			//Check name
			if (_nameList.ContainsKey(name))
				throw new Exception(String.Format("The name {0} is already taken by another background.", name));

			_nameList.Add(name, this);

			_id = id;
			_name = name;
			_texture = tex;

			_backgroundList.Add(this);

			TSDebug.WriteLine("BACKGROUND: \tRegistered new background-> " + this);
		}

		#region Texture Control
		//Returns the proper metadata based on the surrounding blocks
		public virtual byte GetIndexForSurroundings(World w, int x, int y)
		{
			return 0;
		}
		//Returns the path to the texture file
		public virtual string GetTexturePath()
		{
			return @"Textures\Blocks\";
		}
		//Returns the name of the texture file
		public virtual string GetTextureFile()
		{
			return @"ShipBackgrounds";
		}
		#endregion

		public override string ToString()
		{
			return this._name + ":" + this._id;
		}

		#region Static Members
		//List of block names
		private static Dictionary<string, Background> _nameList = new Dictionary<string, Background>();
		//List of block ids
		private static Dictionary<short, Background> _idList = new Dictionary<short, Background>();
		//General list of all added blocks
		private static List<Background> _backgroundList = new List<Background>();
		public static List<Background> BackgroundList { get { return _backgroundList; } }

		//Typecasting an int to a block
		public static implicit operator Background(short s)
		{
			if (!_idList.ContainsKey(s))
				return Background.Empty;

			return _idList[s];
		}
		public static implicit operator short(Background b)
		{
			return b.ID;
		}
		#endregion

		#region Static Background List
		static Background()
		{
			Empty = new Background(0, "Empty", -1)
			{
				ShouldDraw = false
			};
			BasicBulkhead = new Background(1, "Basic Bulkhead", 0);
			Window = new GlassBackground(2, "Window", 1);
		}

		public static readonly Background Empty;
		public static readonly Background BasicBulkhead;
		public static readonly Background Window;
		#endregion
	}
}