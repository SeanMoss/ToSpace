#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using To_Space.GUI;
using To_Space.Managers;
using To_Space.States;
using To_Space.Common;
using To_Space.Terrain;
using To_Space.Terrain.Blocks;
#endregion

namespace To_Space
{
	public class ToSpaceGame : Game
	{
		public static readonly bool DEBUG = true;

		#region Managers
		//State manager
		private StateManager _stateManager;
		public StateManager StateManager { get { return _stateManager; } }
		//Input Manager
		private InputManager _inputManager;
		public InputManager InputManager { get { return _inputManager; } }
		//Thread Manager
		private ThreadManager _threadManager;
		public ThreadManager ThreadManager { get { return _threadManager; } }
		#endregion

		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		public ToSpaceGame()
			: base()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width; //GameSettings.ScreenWidth;
			_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height; //GameSettings.ScreenHeight;
			_graphics.IsFullScreen = true; //GameSettings.FullScreen;
			_graphics.SynchronizeWithVerticalRetrace = true; //GameSettings.VSync;
			_graphics.ApplyChanges();

			TextureManager.InitializeTextures(Content, GraphicsDevice);

			_stateManager = new StateManager(this);
			_inputManager = new InputManager(this);
			_threadManager = new ThreadManager(this);
		}

		protected override void Initialize()
		{
			State state = new MainMenuState();
			state.Game = this;
			state.Initialize();
			state.LoadContent();
			_stateManager.ActiveState = state;

			_stateManager.Initialize();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_stateManager.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

			_inputManager.Update();
			_stateManager.ProcessInput(gameTime);
			_stateManager.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_stateManager.Draw(gameTime);

			base.Draw(gameTime);
		}

		protected override void UnloadContent()
		{
			if (_stateManager.ActiveState is PlayingState)
			{
				PlayingState state = (PlayingState)_stateManager.ActiveState;
				state.ForceWorldSave();
			}

			_threadManager.StopAllThreads();
		}
	}
}
