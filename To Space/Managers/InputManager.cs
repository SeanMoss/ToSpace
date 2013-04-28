using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace To_Space.Managers
{
	public class InputManager
	{
		//Reference to the game using this input manager
		private ToSpaceGame _game;

		//Mouse states
		private MouseState _currentMouseState;
		private MouseState _previousMouseState;

		//Keyboard states
		private KeyboardState _currentKeyState;
		private KeyboardState _previousKeyState;

		//Mouse wheel
		private float _mouseWheelDelta = 0;

		public InputManager(ToSpaceGame game)
		{
			this._game = game;

			_currentMouseState = Mouse.GetState();
			_currentKeyState = Keyboard.GetState();
		}

		public void Update()
		{
			_previousMouseState = _currentMouseState;
			_previousKeyState = _currentKeyState;

			_currentMouseState = Mouse.GetState();
			_currentKeyState = Keyboard.GetState();

			_mouseWheelDelta = _previousMouseState.ScrollWheelValue - _currentMouseState.ScrollWheelValue;
		}

		public bool IsKeyDown(Keys key)
		{
			return _currentKeyState.IsKeyDown(key);
		}

		public bool IsKeyUp(Keys key)
		{
			return _currentKeyState.IsKeyUp(key);
		}

		public bool IsKeyPressed(Keys key)
		{
			return (_currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyUp(key));
		}

		public bool IsKeyReleased(Keys key)
		{
			return (_currentKeyState.IsKeyUp(key) && _previousKeyState.IsKeyDown(key));
		}

		public bool IsMouseButtonDown(MouseButton button)
		{
			if (button == MouseButton.LEFT)
			{
				return _currentMouseState.LeftButton == ButtonState.Pressed;
			}

			if (button == MouseButton.RIGHT)
			{
				return _currentMouseState.RightButton == ButtonState.Pressed;
			}

			return false;
		}

		public bool IsMouseButtonUp(MouseButton button)
		{
			return !IsMouseButtonDown(button);
		}

		public bool IsMouseButtonPressed(MouseButton button)
		{
			if (button == MouseButton.LEFT)
			{
				return (_currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released);
			}
			if (button == MouseButton.RIGHT)
			{
				return (_currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released);
			}

			return false;
		}

		public bool IsMouseButtonReleased(MouseButton button)
		{
			if (button == MouseButton.LEFT)
			{
				return (_currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed);
			}
			if (button == MouseButton.RIGHT)
			{
				return (_currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed);
			}

			return false;
		}

		public Vector2 GetMousePosition()
		{
			return new Vector2(_currentMouseState.X, _currentMouseState.Y);
		}
	}

	public enum MouseButton
	{
		LEFT,
		RIGHT
	}
}