using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace To_Space.Managers
{
	public abstract class Manager
	{
		private ToSpaceGame _game;
		public ToSpaceGame Game { get { return _game; } }

		public Manager(ToSpaceGame game)
		{
			_game = game;
		}

		public abstract void Initialize();
		public abstract void LoadContent();
		public abstract void Update(GameTime gameTime);
	}
}