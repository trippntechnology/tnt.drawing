using System.Drawing;
using TNT.Commons;

namespace TNT.Drawing.Layers;

/// <summary>
/// Provides a visual grid overlay for the drawing surface.
/// Handles property change notifications and optimizes redraws using bitmap caching.
/// </summary>
public class GridLayer : CanvasLayer
{
  // Fields
  private Pen _Pen = new Pen(Color.Black);
  private Bitmap? _Bitmap = null;

  // Properties
  /// <summary>
  /// Gets or sets the color of the grid lines.
  /// Changes to this property trigger a redraw of the grid.
  /// </summary>
  public Color LineColor { get => Get(_Pen.Color); set => Set(value); }

  // Methods
  /// <summary>
  /// Renders the grid onto the provided graphics context.
  /// Regenerates the grid bitmap only when necessary, based on property changes or initial draw.
  /// Draws grid lines at intervals, with thicker lines for major segments.
  /// </summary>
  public override void Draw(Graphics graphics, int snapInterval)
  {
    if (IsVisible)
    {
      base.Draw(graphics, snapInterval);

      if (_backingFieldChanged || _Bitmap == null)
      {
        _Bitmap?.Dispose();
        _Bitmap = new Bitmap(Width, Height);
        using (var g = Graphics.FromImage(_Bitmap))
        {
          var largeSegment = snapInterval * 10;
          _Pen.Color = LineColor;

          for (int x = 0; x < Width; x += snapInterval)
          {
            _Pen.Width = (x % largeSegment == 0) ? 3 : 1;
            g.DrawLine(_Pen, x, 0, x, Height);
          }

          for (int y = 0; y < Height; y += snapInterval)
          {
            _Pen.Width = (y % largeSegment == 0) ? 3 : 1;
            g.DrawLine(_Pen, 0, y, Width, y);
          }
        }

        _backingFieldChanged = false;
      }

      graphics.DrawImage(_Bitmap, 0, 0);
    }
  }

  /// <summary>
  /// Copies relevant properties from another <see cref="CanvasLayer"/> instance.
  /// For <see cref="GridLayer"/>, copies the <see cref="LineColor"/> property.
  /// </summary>
  public override void CopyFrom(CanvasLayer layer)
  {
    base.CopyFrom(layer);

    (layer as GridLayer)?.Also(layer =>
    {
      LineColor = layer.LineColor;
    });
  }
}
