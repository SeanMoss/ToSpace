using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using To_Space.GUI;
using To_Space.Managers;
using To_Space.Terrain;

namespace To_Space.States
{
	public class MainMenuState : State
	{
		private SpriteBatch _spriteBatch;
		private SpriteFont _spriteFont;

		//Music
		private SoundEffect _mainMusic;
		private SoundEffectInstance _mainMusicInstance;
		
		//Main menu parts
		private GuiManager _mainMenu;
		private Button _buttonStartGame;
		private Button _buttonEndGame;
		private Button _buttonDoNothing;

		//Settings menu
		private GuiManager _settingsMenu;

		public MainMenuState()
		{
		}

		public override void Initialize()
		{
			_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
			_mainMenu = new GuiManager(this.Game);
			_mainMenu.Initialize();
		}

		public override void LoadContent()
		{
			_spriteFont = Game.Content.Load<SpriteFont>(@"Fonts\console");

			#region Create Main Menu
			_mainMenu.LoadContent();

			Texture2D _standardTex = Game.Content.Load<Texture2D>(@"GUI\Buttons\StandardBox");
			Texture2D _hoverTex = Game.Content.Load<Texture2D>(@"GUI\Buttons\HoverBox");
			Texture2D _clickTex = Game.Content.Load<Texture2D>(@"GUI\Buttons\ClickedBox");

			_buttonStartGame = new Button("Start Game", new Vector2(100, 100), _standardTex, _mainMenu);
			_buttonDoNothing = new Button("Do Nothing", new Vector2(100, 200), _standardTex, _mainMenu);
			_buttonEndGame = new Button("End Game", new Vector2(100, 300), _standardTex, _mainMenu);

			_buttonStartGame.SetText("Start Game");
			_buttonDoNothing.SetText("This Button Does Something?");
			_buttonEndGame.SetText("End Game");

			_buttonStartGame.TextColor = Color.Green;
			_buttonEndGame.TextColor = Color.Red;

			_buttonStartGame.HoverTexture = _hoverTex;
			_buttonDoNothing.HoverTexture = _hoverTex;
			_buttonEndGame.HoverTexture = _hoverTex;

			_buttonStartGame.ClickTexture = _clickTex;
			_buttonDoNothing.ClickTexture = _clickTex;
			_buttonEndGame.ClickTexture = _clickTex;
			#endregion

			_mainMusic = Game.Content.Load<SoundEffect>(@"Audio\Music\creepy");
			_mainMusicInstance = _mainMusic.CreateInstance();
			_mainMusicInstance.IsLooped = true;
			_mainMusicInstance.Volume = .7f;

			ActivateMainMenu();
		}

		public override void ProcessInput(GameTime gameTime)
		{
			
		}

		public override void Update(GameTime gameTime)
		{
			_mainMenu.Update(gameTime);
			_mainMenu.UpdateMembers();

			if (_mainMusicInstance.State == SoundState.Stopped)
			{
				//
				_mainMusicInstance.Play();
			}

			if (_buttonEndGame.IsClicked)
				Game.Exit();
			if (_buttonStartGame.IsClicked)
			{
				//TODO move to the next menu
				Game.StateManager.ActiveState = Game.StateManager.GetState(typeof(PlayingState));
			}
		}

		public override void Draw(GameTime gameTime)
		{
			Game.GraphicsDevice.Clear(new Color(30, 30, 30));

			_mainMenu.DrawMembers();
		}

		private void ActivateMainMenu()
		{
			_mainMenu.IsActive = true;
		}
	}
}