using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace To_Space.States
{
	public class LoadingState : State
	{
		private SpriteBatch _spriteBatch;
		private SpriteFont _consoleFont;

		//TODO add the requirements for this state

		private bool _loading = false;
		private bool _loaded = false;

		public LoadingState()
		{
		}

		public override void Initialize()
		{
			_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
		}

		public override void LoadContent()
		{
			_consoleFont = Game.Content.Load<SpriteFont>(@"Fonts\console");
		}

		public override void Update(GameTime gameTime)
		{
			if (!_loading)
			{
				_loading = true;
				Thread loadingThread = new Thread(new ThreadStart(this.LoadPlayingState));
				loadingThread.Start();
			}
			if (_loaded)
				StartWorld();
		}

		public override void ProcessInput(GameTime gameTime)
		{
			
		}

		public override void Draw(GameTime gameTime)
		{
			Game.GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin();
			//TODO Draw progress visuals
			_spriteBatch.End();
		}

		private void StartWorld()
		{
			//TODO Switch to the playing state
			Game.Exit();
		}

		private void LoadPlayingState()
		{
			//TODO load world
			Thread.Sleep(2000);

			_loaded = true;
		}
	}
}