using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using To_Space.Managers;

namespace To_Space.Terrain
{
	public sealed class ParallaxingBackground
	{
		//Textures - drawn from 0 to 5
		public Texture2D[] Textures = new Texture2D[5];

		//Vertica Speeds of the textures
		public float[] YScrollSpeeds = new float[5];

		//Horizontal speeds of the textures
		public float[] XScrollSpeeds = new float[5];

		//Vertical offsets of the backgrounds
		public float[] YTextureOffset = new float[5];

		//Horizontal offsets of the backgrounds
		public float[] XTextureOffset = new float[5];

		//Speed modifier for fast scrolling
		public float SpeedModifier = 1f;

		//Speed modifier key
		public Keys FastKey;

		//Movement keys
		public Keys LeftKey, RightKey, UpKey, DownKey;

		//World this belongs to
		private World _world;
		public World World { get { return _world; } }

		//Input manager
		InputManager _inputManager;

		public ParallaxingBackground(World world)
		{
			_world = world;
		}

		public void Update()
		{
			Vector2 move = new Vector2(0);

			if(Keyboard.GetState().IsKeyDown(LeftKey))
			{
				move += new Vector2(-1, 0);
			}
			if (Keyboard.GetState().IsKeyDown(RightKey))
			{
				move += new Vector2(1, 0);
			}
			if (Keyboard.GetState().IsKeyDown(UpKey))
			{
				move += new Vector2(0, -1);
			}
			if (Keyboard.GetState().IsKeyDown(DownKey))
			{
				move += new Vector2(0, 1);
			}

			if (Keyboard.GetState().IsKeyDown(FastKey))
			{
				move *= SpeedModifier;
			}

			int sWidth = _world.Game.GraphicsDevice.DisplayMode.Width;
			int sHeight = _world.Game.GraphicsDevice.DisplayMode.Height;

			for (int i = 0; i < 5; i++)
			{
				XTextureOffset[i] += move.X * XScrollSpeeds[i];
				YTextureOffset[i] += move.Y * YScrollSpeeds[i];

				if (XTextureOffset[i] > sWidth || XTextureOffset[i] < -sWidth)
				{
					XTextureOffset[i] = 0;
				}

				if (YTextureOffset[i] > sHeight || YTextureOffset[i] < -sHeight)
				{
					YTextureOffset[i] = 0;
				}
			}
		}

		public void Draw(SpriteBatch sBatch, Color c)
		{
			int sWidth = _world.Game.GraphicsDevice.DisplayMode.Width;
			int sHeight = _world.Game.GraphicsDevice.DisplayMode.Height;

			sBatch.Begin();

			for (int i = 0; i < 5; i++)
			{
				if (Textures[i] != null)
				{
					for (int x = -sWidth; x <= sWidth; x += sWidth)
					{
						for (int y = -sHeight; y <= sHeight; y += sHeight)
						{
							sBatch.Draw(Textures[i], 
								new Rectangle((int)(x + XTextureOffset[i]), (int)(y + YTextureOffset[i]), sWidth, sHeight), 
								c);
						}
					}
				}
			}

			sBatch.End();
		}

		#region Pre-made Backgrounds

		#endregion
	}
}