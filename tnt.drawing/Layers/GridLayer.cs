﻿using System.Drawing;

namespace TNT.Drawing.Layers;

/// <summary>
/// Represents the grid area of the drawing surface
/// </summary>
public class GridLayer : CanvasLayer
{
  private Pen _Pen = new Pen(Color.Black);

  /// <summary>
  /// The color of the grid lines
  /// </summary>
  public Color LineColor { get => BackingFields.Get(_Pen.Color); set => BackingFields.Set(value); }

  /// Copy constructor
  /// </summary>
  public GridLayer(Canvas canvas) : base(canvas) { }

  /// <summary>
  /// Draws the <see cref="GridLayer"/>
  /// </summary>
  public override void Draw(Graphics graphics)
  {
    if (IsVisible)
    {
      base.Draw(graphics);

      var largeSegment = Canvas.SnapInterval * 10;
      _Pen.Color = LineColor;

      for (int x = 0; x < Width; x += Canvas.SnapInterval)
      {
        _Pen.Width = (x % largeSegment == 0) ? 3 : 1;
        graphics.DrawLine(_Pen, x, 0, x, Height);
      }

      for (int y = 0; y < Height; y += Canvas.SnapInterval)
      {
        _Pen.Width = (y % largeSegment == 0) ? 3 : 1;
        graphics.DrawLine(_Pen, 0, y, Width, y);
      }
    }
  }
}
