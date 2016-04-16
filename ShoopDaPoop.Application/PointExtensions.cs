using System;
using System.Collections.Generic;
using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public static class PointExtensions
	{
		public static void Set(this Point point, Point value)
		{
			point.Set(value.X, value.Y);
		}

		public static Point Subtract(this Point point, Point value)
		{
			return new Point(point.X - value.X, point.Y - value.Y);
		}

		public static Point Add(this Point point, Point value)
		{
			return new Point(point.X + value.X, point.Y + value.Y);
		}

		public static Point Multiply(this Point point, float value)
		{
			return new Point(point.X * value, point.Y * value);
		}

		public static void Normalize(this Point point)
		{
			var val = 1.0f/(float) Math.Sqrt(point.X* point.X + point.Y * point.Y);
			point.X *= val;
			point.Y *= val;
		}

		public static IEnumerable<T> MyDistinct<T>(this IEnumerable<T> enumerable)
		{
			var yielded = new List<T>();
			foreach (var item in enumerable)
			{
				if (yielded.Contains(item)) continue;
				yielded.Add(item);
				yield return item;
			}
		}
	}
}