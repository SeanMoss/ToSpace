using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

using To_Space.Common;
using To_Space.Terrain;
using To_Space.Terrain.Blocks;

namespace To_Space.Items
{
	public static class ColorCycler
	{
		public static World ActiveWorld;

		private static MouseState _mouse = Mouse.GetState();

		public static void Init(World w)
		{
			ActiveWorld = w;
		}

		public static void Update()
		{
			_mouse = Mouse.GetState();

			if (_mouse.LeftButton == ButtonState.Pressed)
			{
				Block b = ActiveWorld.GetBlockAt((int)((_mouse.X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
					(int)((_mouse.Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM));

				if (b is CarpetFloorBlock)
				{
					byte color = ActiveWorld.GetMetadataAt((int)((_mouse.X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
					(int)((_mouse.Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM));

					byte initial = (byte)(color / 16);
					color %= 16;

					ActiveWorld.SetMetadataAt((int)((_mouse.X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
					(int)((_mouse.Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
					ColorManager.GetNextColorAsByte(color));
				}
			}
			if (_mouse.RightButton == ButtonState.Pressed)
			{
				Block b = ActiveWorld.GetBlockAt((int)((_mouse.X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
					(int)((_mouse.Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM));

				if (b is CarpetFloorBlock)
				{
					byte color = ActiveWorld.GetMetadataAt((int)((_mouse.X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
					(int)((_mouse.Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM));

					byte initial = (byte)(color / 16);
					color %= 16;

					ActiveWorld.SetMetadataAt((int)((_mouse.X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
					(int)((_mouse.Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
					(byte)(ColorManager.GetPreviousColorAsByte(color) + (initial * 16)));
				}
			}
		}
	}
}