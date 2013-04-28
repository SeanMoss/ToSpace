using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using To_Space.GUI;

namespace To_Space.Managers
{
	public class GuiManager : Manager
	{
		private List<ClickableObject> _clickableObjects;
		private SpriteBatch _spriteBatch;
		private SpriteFont _guiText;
		public SpriteFont GuiText { get { return _guiText; } }
		public bool IsActive = true;

		public GuiManager(ToSpaceGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			_clickableObjects = new List<ClickableObject>();
			_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
		}

		public override void LoadContent()
		{
			_guiText = Game.Content.Load<SpriteFont>(@"Fonts\console");
		}

		//Could be used to update animated buttons
		public override void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				foreach (ClickableObject obj in _clickableObjects)
				{
					obj.Update(gameTime);
				} 
			}
		}

		//A more general update method
		public void UpdateMembers()
		{
			MouseState _currentMouseState = Mouse.GetState();

			if (this.IsActive)
			{
				foreach (ClickableObject obj in _clickableObjects)
				{
					obj.CheckForClick(_currentMouseState);
				} 
			}
		}

		public void AddManagedClickableObject(ClickableObject co)
		{
			if (!_clickableObjects.Contains(co))
				_clickableObjects.Add(co);
		}

		public void RemoveManagedClickableObject(ClickableObject co)
		{
			if (_clickableObjects.Contains(co))
				_clickableObjects.Remove(co);
		}

		public void RemoveManagedClickableObject(string name)
		{
			foreach (ClickableObject co in _clickableObjects)
			{
				if (name.Equals(co.Name))
				{
					RemoveManagedClickableObject(co);
					break;
				}
			}
		}

		public void DrawMembers()
		{
			if (this.IsActive)
			{
				_spriteBatch.Begin();
				foreach (ClickableObject o in _clickableObjects)
				{
					if (o.IsActive)
						o.Draw(_spriteBatch);
				}
				_spriteBatch.End(); 
			}
		}
	}
}