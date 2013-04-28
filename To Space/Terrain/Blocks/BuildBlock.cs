using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace To_Space.Terrain.Blocks
{
	public abstract class BuildBlock : Block
	{
		public BuildBlock(short id, string name, int tex)
			: base(id, name, tex)
		{

		}
	}
}