using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using To_Space.Common;

namespace To_Space.Terrain.Blocks
{
	public class HullBlock : BuildBlock
	{
		public HullBlock(short id, string name, int tex)
			: base(id, name, tex)
		{
			this.UpdateOnNeighborChange = true;
		}

		//0 is open around, 1 is single up, 2 is single right, 3 is single down, 4 is single left,
		//5 is corner top/right, 6 is corner right/down, 7 is corner down/left, 8 is corner left/up,
		//9 is 2 across, 10 is 2 up/down, 11 is 3 pointing up, 12 is 3 pointing right, 13 is 3 pointing down
		//14 is 9 pointing left, 15 is all connected
		#region Metadata Overriding
		public override byte GetMetadataForSurroundings(World w, int x, int y)
		{
			bool l = (w.GetBlockAt(x - 1, y)) is HullBlock;
			bool r = (w.GetBlockAt(x + 1, y)) is HullBlock;
			bool u = (w.GetBlockAt(x, y - 1)) is HullBlock;
			bool d = (w.GetBlockAt(x, y + 1)) is HullBlock;

			if (l)
			{
				if (r)
				{
					if (u)
					{
						if (d) // L R U D
						{
							return 15;
						}
						else // L U R
						{
							return 11;
						}
					}
					else if (d) // L D R
					{
						return 13;
					}
					else // L R
					{
						return 9;
					}
				}
				else if (u)
				{
					if (d) // L U D
					{
						return 14;
					}
					else // L U
					{
						return 8;
					}
				}
				else if (d) // L D
				{
					return 7;
				}
				else // L
				{
					return 4;
				}
			}
			else if (u)
			{
				if (r)
				{
					if (d) // U R D
					{
						return 12;
					}
					else // U R
					{
						return 5;
					}
				}
				else if (d) // U D
				{
					return 10;
				}
				else // U
				{
					return 1;
				}
			}
			else if (r)
			{
				if (d) // R D
				{
					return 6;
				}
				else // R
				{
					return 2;
				}
			}
			else if (d) // D
			{
				return 3;
			}

			return 0; //Nothing
		}

		public override int GetIndexBasedOnMetadata(byte meta)
		{
			return meta;
		}
		#endregion
	}
}