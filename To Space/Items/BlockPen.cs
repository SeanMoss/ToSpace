using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using To_Space.Terrain;
using To_Space.Terrain.Blocks;
using To_Space.Terrain.Backgrounds;
using To_Space.Common;
using To_Space.Managers;

namespace To_Space.Items
{
	public static class BlockPen
	{
		//World being edited
		public static World ActiveWorld;

		//Block to replace the clicked area
		public static Block ActiveBlock = Block.SquareHull;

		public static void Init(World world)
		{
			ActiveWorld = world;
		}

		public static void Update()
		{
			InputManager input = ActiveWorld.Game.InputManager;

			bool left = input.IsMouseButtonDown(MouseButton.LEFT);
			bool right = input.IsMouseButtonDown(MouseButton.RIGHT);

			if (left)
			{
				if(input.IsKeyDown(Keys.Q))
				{
					ActiveWorld.SetBackgroundAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
						(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
						Background.BasicBulkhead);
				}
				else
				{
					ActiveWorld.SetBlockAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
						(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
						ActiveBlock);
				}
			}
			if (right)
			{
				Block b = ActiveWorld.GetBlockAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
						(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM));

				if (input.IsKeyDown(Keys.Q))
				{
					ActiveWorld.SetBackgroundAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
						(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
						Background.Window);
				}
				else
				{
					if (b != Block.CarpetFloor)
					{
						ActiveWorld.SetBlockAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
							(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
							Block.CarpetFloor);
					}
					else
					{
						if (input.IsMouseButtonPressed(MouseButton.RIGHT))
						{
							byte color = ActiveWorld.GetMetadataAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
											(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM));

							TSDebug.WriteLine("BLOCKPEN: Changing the color of a carpet block from " + color + " to " + (color + 1));

							ActiveWorld.SetBlockAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
								(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
								Block.CarpetFloor,
								ColorManager.GetNextColorAsByte(color)); 
						}
					}
				}
			}

			if (input.IsKeyDown(Keys.X))
			{
				ActiveWorld.SetBlockAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
					(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
					Block.Vacuum, 0);

				if (input.IsKeyDown(Keys.Z))
				{
					ActiveWorld.SetBackgroundAt((int)((input.GetMousePosition().X + ActiveWorld.Camera.Offset.X) / Block.BLOCK_DIM),
						(int)((input.GetMousePosition().Y + ActiveWorld.Camera.Offset.Y) / Block.BLOCK_DIM),
						Background.Empty);
				}
			}
		}
	}
}