using System.Drawing;

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

  /// <summary>
  /// Indicates whether the grid layer needs to be repainted.
  /// - Initialized to <c>true</c> so the grid is drawn on first render.
  /// - Set to <c>true</c> whenever a property managed by <see cref="_BackingFields"/> changes (e.g., color, dimensions, visibility).
  /// - Used in <see cref="Draw"/> to determine if the grid bitmap should be regenerated.
  /// - Reset to <c>false</c> after redrawing, so the grid is only regenerated when necessary.
  /// </summary>
  private bool _Redraw = true;

  // Properties
  /// <summary>
  /// Gets or sets the color of the grid lines.
  /// Changes to this property trigger a redraw of the grid.
  /// </summary>
  public Color LineColor
  {
    get => _BackingFields.Get(_Pen.Color);
    set => _BackingFields.Set(value);
  }

  // Constructors
  /// <summary>
  /// Initializes a new instance of <see cref="GridLayer"/>.
  /// Subscribes to property change notifications to trigger grid redraws when properties change.
  /// </summary>
  public GridLayer()
  {
    _BackingFields.OnFieldChanged += (field, value) => { _Redraw = true; };
  }

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

      if (_Redraw || _Bitmap == null)
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

        _Redraw = false;
      }

      graphics.DrawImage(_Bitmap, 0, 0);
    }
  }
}
