using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace To_Space.Terrain.Materials
{
	public class Material
	{
		#region Properties
		//The name of this material
		private string _name;
		public string Name { get { return _name; } }
		//The id of the material
		private int _id;
		public int ID { get { return _id; } }

		#region Fields
		//If this material can support entities
		public bool Solid = true;

		#region Damage Fields
		#endregion
		#endregion
		#endregion

		public Material(int id, string name)
		{
			if (_idList.ContainsKey(id))
				throw new Exception("There is already a material with id: " + id);

			_id = id;

			if (_nameList.ContainsKey(name))
				throw new Exception("There is already a material with name: " + name);

			_name = name;

			if (_matList.Contains(this))
				throw new Exception("The material " + this + " has already been registered.");

			_matList.Add(this);

			TSDebug.WriteLine("MATERIAL: \tRegistered new material-> " + this);
		}

		public override string ToString()
		{
			return _name + ":" + _id;
		}

		#region Statics
		//Id list
		private Dictionary<int, Material> _idList = new Dictionary<int, Material>();
		//Name list
		private Dictionary<string, Material> _nameList = new Dictionary<string, Material>();
		//Material list
		private List<Material> _matList = new List<Material>();
		#endregion

		#region Materials
		static Material()
		{
			Gas = new Material(0, "Gas")
			{
				Solid = false
			};
			Metal = new Material(1, "Solid")
			{
				
			};
		}

		public static readonly Material Gas;
		//Temporary solid material
		public static readonly Material Metal;
		#endregion
	}
}