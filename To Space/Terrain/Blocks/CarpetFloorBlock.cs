using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using To_Space.Common;

namespace To_Space.Terrain.Blocks
{
	public class CarpetFloorBlock : BuildBlock
	{
		public CarpetFloorBlock(short id, string name, int tex)
			: base(id, name, tex)
		{
			this.UpdateOnNeighborChange = true;
		}

		#region Overrides
		public override byte GetMetadataForSurroundings(World w, int x, int y)
		{
			bool l = (w.GetBlockAt(x - 1, y)) == Block.Air || (w.GetBlockAt(x - 1, y)) == Block.Vacuum;
			bool r = (w.GetBlockAt(x + 1, y)) == Block.Air || (w.GetBlockAt(x + 1, y)) == Block.Vacuum;

			byte color = (byte)(w.GetMetadataAt(x, y) % 16);

			if (!(l || r)) //Return a connected carpet
			{
				return color;
			}
			if (l && !r) //Return a carpet only connected on the right, start shifted down one row
			{
				return (byte)(color + 16);
			}
			if (!l && r) //Return a carpet only connected on the left, start shifted down two rows
			{
				return (byte)(color + 32);
			}

			return color;
		}

		public override void OnBlockPlaced(int x, int y, World w)
		{
			w.SetMetadataAt(x, y, 0, false);
			//w.SetMetadataAt(x, y, this.GetMetadataForSurroundings(w, x, y), false);
		}

		public override int GetIndexBasedOnMetadata(byte meta)
		{
			return (int)meta + 16;
		}

		public override void UpdateNeighbors(int x, int y, World world)
		{
			bool l = world.GetBlockAt(x - 1, y) == Block.CarpetFloor;
			bool r = world.GetBlockAt(x + 1, y) == Block.CarpetFloor;

			if (l)
			{
				world.SetMetadataAt(x - 1, y, this.GetMetadataForSurroundings(world, x - 1, y), false);
			}
			if (r)
			{
				world.SetMetadataAt(x + 1, y, this.GetMetadataForSurroundings(world, x + 1, y), false);
			}

			base.UpdateNeighbors(x, y, world);
		}

		public override void OnBlockRemoved(int x, int y, World w)
		{
			this.UpdateNeighbors(x, y, w);
		}
		#endregion
	}
}