using System.Drawing;

namespace TNT.Drawing.Layers;

/// <summary>
/// Represents the grid area of the drawing surface
/// </summary>
public class GridLayer : CanvasLayer
{
  private Pen _Pen = new Pen(Color.Black);
  private Bitmap? _Bitmap = null;

  /// <summary>
  /// The color of the grid lines
  /// </summary>
  public Color LineColor { get => _BackingFields.Get(_Pen.Color); set => _BackingFields.Set(value); }

  /// Copy constructor
  /// </summary>
  public GridLayer(Canvas canvas) : base(canvas) { }

  /// <summary>  
  /// Renders the grid layer onto the provided graphics context.  
  /// This includes drawing grid lines based on the canvas's snap interval and visibility settings.  
  /// </summary>  
  public override void Draw(Graphics graphics)
  {
    if (IsVisible)
    {
      base.Draw(graphics);

      if (_Redraw || _Bitmap == null)
      {
        _Bitmap?.Dispose();
        _Bitmap = new Bitmap(Width, Height);
        using (var g = Graphics.FromImage(_Bitmap))
        {
          var largeSegment = Canvas.SnapInterval * 10;
          _Pen.Color = LineColor;

          for (int x = 0; x < Width; x += Canvas.SnapInterval)
          {
            _Pen.Width = (x % largeSegment == 0) ? 3 : 1;
            g.DrawLine(_Pen, x, 0, x, Height);
          }

          for (int y = 0; y < Height; y += Canvas.SnapInterval)
          {
            _Pen.Width = (y % largeSegment == 0) ? 3 : 1;
            g.DrawLine(_Pen, 0, y, Width, y);
          }
        }

        _Redraw = false;
      }

      graphics.DrawImage(_Bitmap, 0, 0);
    }
  }
}
