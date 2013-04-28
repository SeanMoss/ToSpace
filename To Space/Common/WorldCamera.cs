using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using To_Space.Entities;
using To_Space.Terrain;
using To_Space.Terrain.Blocks;

namespace To_Space.Common
{
	public class WorldCamera
	{
		//Keeps track of the position of the camera to offset the world
		private Vector2 _offset = Vector2.Zero;
		public Vector2 Offset { get { return _offset; } }

		//The entity that this camera is locked on
		private Entity _focus;
		public Entity FocusedEntity { get { return _focus; } }

		//World
		private World _world;
		public World World { get { return _world; } }

		public WorldCamera(World w)
		{
			_world = w;
		}

		public void Update()
		{
			if (_focus != null)
			{
				int sWidth = _world.Game.GraphicsDevice.DisplayMode.Width / 2;
				int sHeight = _world.Game.GraphicsDevice.DisplayMode.Height / 2;

				_offset = new Vector2(_focus.EntityCenter.X - sWidth, _focus.EntityCenter.Y - sHeight);
			}
		}

		public void Move(int x, int y)
		{
			if (_focus == null)
			{
				_offset += new Vector2(x, y);
				_offset.X = MathHelper.Clamp(_offset.X, 0, _world.RegionWidth * Block.BLOCK_DIM * Region.REGION_DIM);
				_offset.Y = MathHelper.Clamp(_offset.Y, 0, _world.RegionHeight * Block.BLOCK_DIM * Region.REGION_DIM); 
			}
		}

		public void MoveTo(int x, int y)
		{
			if (_focus == null)
			{
				_offset = new Vector2(x, y);
				_offset.X = MathHelper.Clamp(_offset.X, 0, _world.RegionWidth * Block.BLOCK_DIM * Region.REGION_DIM);
				_offset.Y = MathHelper.Clamp(_offset.Y, 0, _world.RegionHeight * Block.BLOCK_DIM * Region.REGION_DIM); 
			}
		}

		public void SetFocusOn(Entity e)
		{
			if (_focus != null)
			{
				_focus.SetFocus(false); 
			}

			_focus = e;

			e.SetFocus(true);
		}
	}
}