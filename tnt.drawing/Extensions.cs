﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TNT.Drawing
{
	/// <summary>
	/// Extension methods
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Kotlin let
		/// </summary>
		public static R Let<T, R>(this T value, Func<T, R> block)
		{
			if (value == null) return default;
			return block(value);
		}

		/// <summary>
		/// Kotlin also
		/// </summary>
		public static T Also<T>(this T value, Action<T> block)
		{
			if (value == null) return default;
			block(value);
			return value;
		}

		/// <summary>
		/// Transforms a <see cref="Point"/> from <see cref="CoordinateSpace.Page"/> to <see cref="CoordinateSpace.World"/>
		/// with the current <paramref name="graphics"/>
		/// </summary>
		public static Point ToGridCoordinates(this Point value, Graphics graphics)
		{
			var points = new Point[] { value };
			graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Page, points);
			return points[0];
		}

		/// <summary>
		/// Transforms a <see cref="Point"/> from <see cref="CoordinateSpace.World"/> to <see cref="CoordinateSpace.Page"/>
		/// with the current <paramref name="graphics"/>
		/// </summary>
		public static Point ToCanvasCoordinates(this Point value, Graphics graphics)
		{
			var points = new Point[] { value };
			graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, points);
			return points[0];
		}

		/// <summary>
		/// Subtracts the <see cref="Point"/> p2 from <see cref="Point"/> p1
		/// </summary>
		public static Point Subtract(this Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);

		public static Point Snap(this Point point, int snap)
		{
			var modX = point.X % snap;
			var modY = point.Y % snap;
			var newX = point.X / snap * snap;
			var newY = point.Y / snap * snap;
			point.X = modX >= snap / 2 ? newX + snap : newX;
			point.Y = modY >= snap / 2 ? newY + snap : newY;
			return point;
		}
	}
}
