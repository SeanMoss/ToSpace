using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace To_Space.Common
{
	public enum ToSpaceColor
	{
		WHITE = 0,
		BLUE = 1,
		RED = 2,
		YELLOW = 3,
		GREEN = 4,
		ORANGE = 5,
		PURPLE = 6,
		CYAN = 7,
		PINK = 8,
		LIGHTGREY = 9,
		GREY = 10,
		DARKGREY = 11,
		BROWN = 12,
		LIGHTGREEN = 13,
		TAN = 14,
		BLACK = 15
	}

	public static class ColorManager
	{
		public static byte GetNextColorAsByte(byte b)
		{
			byte row = (byte)(b / 16);

			b += 1;

			b %= 16;

			return (byte)(b + (row * 16));
		}

		public static byte GetPreviousColorAsByte(byte b)
		{
			byte row = (byte)(b / 16);

			b -= 1;

			b %= 16;

			return (byte)(b + (row * 16));
		}
	}
}