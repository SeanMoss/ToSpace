using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using To_Space.Terrain;
using To_Space.Terrain.Blocks;

namespace To_Space.States
{
	public class PlayingState : State
	{
		//World being played on
		private World _world;
		public World World { get { return _world; } }

		//Graphics
		private SpriteBatch _spriteBatch;

		public PlayingState()
		{

		}

		public override void Initialize()
		{
			_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			_world = new World(10, 10, "Test", Game);
		}

		public override void LoadContent()
		{
			
		}

		public override void ProcessInput(GameTime gameTime)
		{
			if (Game.InputManager.IsKeyPressed(Keys.Delete))
			{
				ReturnToMainMenu();
			}

			//if (Game.InputManager.IsKeyPressed(Keys.P))
			//{
			//	_world.SaveFullMapPicture(_spriteBatch);
			//}
		}

		public override void Update(GameTime gameTime)
		{
			if (_world != null)
			{
				_world.Update(gameTime); 
			}
		}

		public override void Draw(GameTime gameTime)
		{
			Game.GraphicsDevice.Clear(Color.CornflowerBlue);

			if (_world != null)
			{
				_world.Draw(_spriteBatch, gameTime); 
			}
		}

		public void ForceWorldSave()
		{
			if (_world != null)
			{
				_world.ForceWorldSave(true); 
			}
		}

		public void ReturnToMainMenu()
		{
			_world.ForceWorldSave(false);
			_world = null;
			Game.StateManager.ActiveState = Game.StateManager.GetState(typeof(MainMenuState));
		}
	}
}