using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace TNT.Drawing.Layers
{
	/// <summary>
	/// Represents the grid area of the drawing surface
	/// </summary>
	public class GridLayer : CanvasLayer
	{
		private Pen _Pen = new Pen(Color.Black);
		private SolidBrush _ShadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black));


		/// <summary>
		/// The number of pixels between the line in the grid
		/// </summary>
		public int PixelsPerSegment { get => BackingFields.Get(10); set => BackingFields.Set(value); }

		/// <summary>
		/// The color of the grid lines
		/// </summary>
		public Color LineColor { get => BackingFields.Get(_Pen.Color); set => BackingFields.Set(value); }

		/// <summary>
		/// The shadow color
		/// </summary>
		public Color ShadowColor { get => BackingFields.Get(_ShadowBrush.Color); set => BackingFields.Set(value); }


		public GridLayer() : base() { }

		/// <summary>
		/// Initializes the <see cref="GridLayer"/>
		/// </summary>
		public GridLayer(Color lineColor, int pixelsBetweenLines)
		{
			PixelsPerSegment = pixelsBetweenLines;
			LineColor = lineColor;
		}

		protected override void DrawImage(Graphics graphics)
		{
			var largeSegment = PixelsPerSegment * 10;
			_Pen.Color = LineColor;

			for (int x = 0; x < Width; x += PixelsPerSegment)
			{
				_Pen.Width = (x % largeSegment == 0) ? 3 : 1;
				graphics.DrawLine(_Pen, x, 0, x, Height);
			}

			for (int y = 0; y < Height; y += PixelsPerSegment)
			{
				_Pen.Width = (y % largeSegment == 0) ? 3 : 1;
				graphics.DrawLine(_Pen, 0, y, Width, y);
			}

			base.DrawImage(graphics);
		}
	}
}
