using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace To_Space.Common
{
	//All inner angles must be < 180 degrees
	public struct BoundingPolygon
	{
		public List<Vector2> Points;

		public BoundingPolygon(params Vector2[] p)
		{
			Points = new List<Vector2>();

			foreach (Vector2 point in p)
			{
				Points.Add(point);
			}
		}

		public void AddPoint(Vector2 p)
		{
			Points.Add(p);
		}

		public bool Intersects(BoundingPolygon b)
		{
			BoundingPolygon a = this;

			foreach (var boundingBox in new[] { a, b })
			{
				for (int i1 = 0; i1 < boundingBox.Points.Count; i1++)
				{
					int i2 = (i1 + 1) % boundingBox.Points.Count;
					var p1 = boundingBox.Points[i1];
					var p2 = boundingBox.Points[i2];

					var normal = new Vector2(p2.Y - p1.Y, p1.X - p2.X);

					double? minA = null, maxA = null;
					foreach (var p in a.Points)
					{
						var projected = normal.X * p.X + normal.Y * p.Y;
						if (minA == null || projected < minA)
							minA = projected;
						if (maxA == null || projected > maxA)
							maxA = projected;
					}

					double? minB = null, maxB = null;
					foreach (var p in b.Points)
					{
						var projected = normal.X * p.X + normal.Y * p.Y;
						if (minB == null || projected < minB)
							minB = projected;
						if (maxB == null || projected > maxB)
							maxB = projected;
					}

					if (maxA < minB || maxB < minA)
						return false;
				}
			}
			return true;
		}

		#region Operator Overloading
		//Adding and sbutracting vector2s
		public static BoundingPolygon operator +(BoundingPolygon poly, Vector2 other)
		{
			BoundingPolygon copy = poly;

			for (int i = 0; i < copy.Points.Count(); i++)
			{
				Vector2 vec = copy.Points[i];

				vec.X += other.X;
				vec.Y += other.Y;

				copy.Points[i] = vec;
			}

			return copy;
		}
		public static BoundingPolygon operator -(BoundingPolygon poly, Vector2 other)
		{
			BoundingPolygon copy = poly;

			for (int i = 0; i < copy.Points.Count(); i++)
			{
				Vector2 vec = copy.Points[i];

				vec.X -= other.X;
				vec.Y -= other.Y;

				copy.Points[i] = vec;
			}

			return copy;
		}
		#endregion
	}
}