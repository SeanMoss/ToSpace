using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace To_Space.Items
{
	public class Item
	{
		#region Fields
		#region Properties
		//Id of the item
		private short _id;
		public short ID { get { return _id; } }

		//Name of the item
		private string _name;
		public string Name { get { return _name; } }
		#endregion

		//Max damage of the item before breaking
		public short MaxDamage = 256;
		#endregion

		public Item(short id, string name)
		{
			if (_idList.ContainsKey(id))
				throw new Exception("The id " + id + " is already being used by an item.");

			_idList.Add(id, this);

			if (_nameList.ContainsKey(name))
				throw new Exception("The name " + name + " is already being used by an item.");

			_nameList.Add(name, this);

			this._id = id;
			this._name = name;
		}

		#region Static Members
		//List of used IDs
		private static Dictionary<short, Item> _idList = new Dictionary<short, Item>();
		//List of used names
		private static Dictionary<string, Item> _nameList = new Dictionary<string, Item>();

		//List of Items
		private static List<Item> _itemList = new List<Item>();
		public static List<Item> ItemList { get { return _itemList; } }

		#region Static Overrides
		public static implicit operator Item(short s)
		{
			if (!_idList.ContainsKey(s))
				throw new Exception("There is no block with the id " + s);

			return _idList[s];
		}
		public static implicit operator short(Item i)
		{
			return i.ID;
		}
		#endregion
		#endregion
	}
}