using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace To_Space.States
{
	//A class used to differentiate between game states and update and draw accordingly
	public abstract class State
	{
		private ToSpaceGame _game;
		public ToSpaceGame Game
		{
			get { return _game; }
			set { _game = value; }
		}

		public State()
		{
		}

		public abstract void Initialize();
		public abstract void LoadContent();
		public abstract void ProcessInput(GameTime gameTime);
		public abstract void Update(GameTime gameTime);
		public abstract void Draw(GameTime gameTime);
	}
}