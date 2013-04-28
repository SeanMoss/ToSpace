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
	public abstract class ClickableObject
	{
		private GuiManager _parent;
		public GuiManager Parent { get { return _parent; } }
		private string _name;
		public string Name { get { return _name; } }
		public bool IsActive = true;

		public bool IsClicked { get; protected set; }
		private Vector2 _position;
		public Vector2 Position
		{
			get { return _position; }
			set
			{
				_position = value;
				BoundingBox = new Rectangle((int)_position.X, (int)_position.Y, (int)_width, (int)_height);
			}
		}
		public Rectangle BoundingBox { get; private set; }
		private float _width = 10;
		public float Width
		{
			get { return _width; }
			set
			{
				_width = value;
				UpdateBoundingBox();
			}
		}
		private float _height = 10;
		public float Height
		{
			get { return _height; }
			set
			{
				_height = value;
				UpdateBoundingBox();
			}
		}

		public ClickableObject(Vector2 position, string name, GuiManager manager)
		{
			Position = position;
			_name = name;
			_parent = manager;
			_parent.AddManagedClickableObject(this);
		}

		public void SetClicked(bool click)
		{
			this.IsClicked = click;
		}

		public void UpdateBoundingBox()
		{
			this.BoundingBox = new Rectangle((int)_position.X, (int)_position.Y, (int)_width, (int)_height);
		}

		public abstract void CheckForClick(MouseState mouseState);
		public abstract void Update(GameTime gameTime);
		public abstract void Draw(SpriteBatch spriteBatch); //Do not call SpriteBatch.Begin() in this method
	}
}