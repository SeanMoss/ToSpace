using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace To_Space.Terrain.Backgrounds
{
	public class GlassBackground : Background
	{
		public GlassBackground(short id, string name, int tex)
			: base(id, name, tex)
		{

		}

		#region Overrides
		public override byte GetIndexForSurroundings(World w, int x, int y)
		{
			bool l = w.GetBackgroundAt(x - 1, y) == this;
			bool r = w.GetBackgroundAt(x + 1, y) == this;
			bool u = w.GetBackgroundAt(x, y - 1) == this;
			bool d = w.GetBackgroundAt(x, y + 1) == this;

			if (l && r && u && d) //All sides glass
			{
				return 10;
			}
			if (l && r && d && !u) // L R D
			{
				return 2;
			}
			if (l && r && u && !d) // L R U
			{
				return 4;
			}
			if (!l && u && d && r) // R U D
			{
				return 5;
			}
			if (l && u && d && !r) // L U D
			{
				return 3;
			}
			if (!l && u && !d && r) // U R
			{
				return 6;
			}
			if (!l && !u && d && r) // D R
			{
				return 7;
			}
			if (l && !u && d && !r) // L D
			{
				return 8;
			}
			if (l && u && !d && !r) // L U
			{
				return 9;
			}

			return 1;
		}
		#endregion
	}
}