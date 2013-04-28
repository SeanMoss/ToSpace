using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using To_Space.Managers;

namespace To_Space.GUI
{
	//Can be treated as a one time press button, or a toggle button
	public class Button : ClickableObject
	{
		public Texture2D HoverTexture;
		public Texture2D ClickTexture;
		public Texture2D DeactivatedTexture;
		private string _buttonText;
		public string ButtonText { get { return _buttonText; } }
		public Color TextColor = Color.White;
		public Texture2D StandardTexture;
		private Texture2D _activeTexture;

		private bool _beingPressed = false;
		private bool BeingPressed
		{
			get { return _beingPressed; }
			set
			{
				_beingPressed = value;
				if (_beingPressed && ClickTexture != null)
				{
					this._activeTexture = ClickTexture;
				}
				else
				{
					this._activeTexture = StandardTexture;
				}
			}
		}

		public Button(string name, Vector2 vec, Texture2D tex, GuiManager manager)
			: base(vec, name, manager)
		{
			StandardTexture = tex;
			_activeTexture = tex;
			this.SetText("Button Text Not Set");
		}

		public void SetText(string newText)
		{
			this._buttonText = newText;
			this.Width = this.Parent.GuiText.MeasureString(newText).X + 50f;
			this.Height = this.Parent.GuiText.MeasureString(newText).Y + 20f;
		}

		public override void CheckForClick(MouseState mouseState)
		{
			if (this.IsActive && this.Parent.IsActive)
			{
				//If the button is not currently set as clicked
				if (!this.IsClicked)
				{
					//Cancel a button being pressed if the mouse isn't over it
					if (mouseState.LeftButton == ButtonState.Pressed && this.BeingPressed && 
						!BoundingBox.Contains(new Point(mouseState.X, mouseState.Y)))
					{
						this.BeingPressed = false;
						return;
					}

					//Releasing the button after being pressed and setting the button to clicked
					if (mouseState.LeftButton == ButtonState.Released && this.BeingPressed && 
						BoundingBox.Contains(new Point(mouseState.X, mouseState.Y)))
					{
						this.SetClicked(true);
						this.BeingPressed = false;
						_activeTexture = ClickTexture;
						return;
					}

					//Press the button, but dont set it clicked until the mouse is released
					if (mouseState.LeftButton == ButtonState.Pressed && !this.BeingPressed && 
						BoundingBox.Contains(new Point(mouseState.X, mouseState.Y)))
					{
						this.BeingPressed = true;
						return;
					}
					
					//Hovering control
					//Check if the mouse is hovering over the button
					if (mouseState.LeftButton == ButtonState.Released && !this.BeingPressed &&
						BoundingBox.Contains(new Point(mouseState.X, mouseState.Y)) && this.HoverTexture != null)
					{
						this._activeTexture = HoverTexture;
						return;
					}

					//Check if the mouse is no longer hovering over the button
					if (mouseState.LeftButton == ButtonState.Released && !this.BeingPressed &&
						!BoundingBox.Contains(new Point(mouseState.X, mouseState.Y)) && this.HoverTexture != null)
					{
						this._activeTexture = StandardTexture;
						return;
					}
				}
				//If the button is currently set as clicked
				else
				{
					//Press the button if it is clicked and alread pressed
					if (mouseState.LeftButton == ButtonState.Pressed && !this.BeingPressed && this.IsClicked &&
						BoundingBox.Contains(new Point(mouseState.X, mouseState.Y)))
					{
						this.BeingPressed = true;
					}

					//Cancel a button being pressed if the mouse isn't over it
					if (mouseState.LeftButton == ButtonState.Pressed && this.BeingPressed &&
						!BoundingBox.Contains(new Point(mouseState.X, mouseState.Y)))
					{
						this.BeingPressed = false;
						return;
					}

					//Releasing the button after being pressed and setting the button to not clicked
					if (mouseState.LeftButton == ButtonState.Released && this.BeingPressed &&
						BoundingBox.Contains(new Point(mouseState.X, mouseState.Y)))
					{
						this.SetClicked(false);
						this.BeingPressed = false;
						this._activeTexture = StandardTexture;
						return;
					}
				}
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (!this.IsActive && this.DeactivatedTexture != null)
			{
				this._activeTexture = DeactivatedTexture;
			}
			else if (this.IsActive && !this.IsClicked && !this.BeingPressed)
			{
				this._activeTexture = StandardTexture;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(_activeTexture, BoundingBox, Color.White);
			spriteBatch.DrawString(Parent.GuiText, ButtonText, new Vector2(this.Position.X + 25f, this.Position.Y + 10f), TextColor);
		}
	}
}