using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace To_Space
{
	public static class TSDebug
	{
		public static void WriteLine(string s)
		{
			if (ToSpaceGame.DEBUG)
			{
				Console.WriteLine(s);
			}
		}
	}
}