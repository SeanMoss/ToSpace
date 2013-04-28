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
	public class SplashState : State
	{
		private SpriteBatch _spriteBatch;
		private Texture2D _rmSplash;
		private Texture2D _mgSplash;
		private Texture2D _logoSplash;
		private Texture2D[] _textureList = new Texture2D[3];
		private Texture2D _cover;

		private int _activeTexture = 0;
		private int _splashTime = 3000;
		private int _fadeTime = 800;

		private float _alpha = 0;
		private bool _updating = false;

		public SplashState()
		{
		}

		public override void Initialize()
		{
			_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
		}

		public override void LoadContent()
		{
			_rmSplash = Game.Content.Load<Texture2D>(@"Splashes\RockMossLogo");
			_mgSplash = Game.Content.Load<Texture2D>(@"Splashes\MonogameSplash");
			_logoSplash = Game.Content.Load<Texture2D>(@"Splashes\ToSpaceLogo");

			_textureList[0] = _rmSplash;
			_textureList[1] = _mgSplash;
			_textureList[2] = _logoSplash;

			_cover = new Texture2D(Game.GraphicsDevice, 1, 1);
			_cover.SetData<Color>(new Color[] { new Color(30, 30, 30) } );
		}

		public override void ProcessInput(GameTime gameTime)
		{
			//Debug system
			if (Game.InputManager.IsKeyDown(Keys.Space))
				goToMainMenu();
		}

		Thread splashThread;
		public override void Update(GameTime gameTime)
		{
			if (!_updating && _activeTexture > 2)
				goToMainMenu();

			if (!_updating)
			{
				_updating = true;
				splashThread = new Thread(new ThreadStart(this.cycleOneSplash));
				splashThread.Name = "Splash State Thread";
				Game.ThreadManager.AddThread(splashThread);
				splashThread.Start();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			Game.GraphicsDevice.Clear(new Color(30, 30, 30));

			Texture2D _tex = _textureList[_activeTexture];
			Vector2 _dim = new Vector2(Game.GraphicsDevice.PresentationParameters.BackBufferWidth, Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

			_spriteBatch.Begin();
			_spriteBatch.Draw(_tex, new Vector2((_dim.X / 2) - (_tex.Width / 2), (_dim.Y / 2) - (_tex.Height / 2)), Color.White);
			_spriteBatch.Draw(_cover, new Rectangle(0, 0, (int)_dim.X, (int)_dim.Y), new Color(255, 255, 255, (1 - _alpha)));
			_spriteBatch.End();
		}

		private void cycleOneSplash()
		{
			while (_alpha < 1) //Fade the splash in
			{
				Thread.Sleep(_fadeTime / 50);
				_alpha += (1 / 50f);
			}
			_alpha = 1f;

			Thread.Sleep(_splashTime); //Keep the splash on screen

			while (_alpha > 0) //Fade the splash out
			{
				Thread.Sleep(_fadeTime / 50);
				_alpha -= (1 / 50f);
			}
			_alpha = 0f;

			//Move to next texture
			_activeTexture++;
			_updating = false;
		}

		private void goToMainMenu()
		{
			Game.ThreadManager.RemoveThread(splashThread);
			Game.StateManager.ActiveState = Game.StateManager.GetState(typeof(MainMenuState));
		}
	}
}