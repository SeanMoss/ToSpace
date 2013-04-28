using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace To_Space.Common
{
	public struct Vector2i
	{
		public int X;
		public int Y;

		public Vector2i(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector2i)
			{
				Vector2i other = (Vector2i)obj;
				return (this.X == other.X && this.Y == other.Y);
			}

			return base.Equals(obj);
		}

		public static bool operator ==(Vector2i a, Vector2i b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Vector2i a, Vector2i b)
		{
			return !(a.X == b.X && a.Y == b.Y);
		}

		public static Vector2i operator +(Vector2i a, Vector2i b)
		{
			return new Vector2i(a.X + b.X, a.Y + b.Y);
		}

		public static Vector2i operator -(Vector2i a, Vector2i b)
		{
			return new Vector2i(a.X - b.X, a.Y - b.Y);
		}

		public override int GetHashCode()
		{
			int hash = 23;
			unchecked
			{
				hash = hash * 37 + X;
				hash = hash * 37 + Y;
			}
			return hash;
		}

		public override string ToString()
		{
			return ("vector2i (" + X + "," + Y + ")");
		}
	}
}