#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

using To_Space.Common;
#endregion

namespace To_Space
{
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		private static ToSpaceGame game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			GameSettings.Initialize();
			game = new ToSpaceGame();
			game.Run();
		}
	}
}
