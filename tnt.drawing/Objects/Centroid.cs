using System.Drawing;
using System.Drawing.Drawing2D;
using TNT.Drawing.Extensions;

namespace TNT.Drawing.Objects;

/// <summary>
/// Represents a centroid, drawn as a transparent outlined circle with cross hairs.
/// </summary>
public class Centroid : Vertex
{
  private static readonly SolidBrush _brush = new(Color.Blue);

  // Color constants for drawing
  private static readonly Color OutlineColor = Color.FromArgb(128, Color.Black);
  private static readonly Color CrosshairColor = OutlineColor;

  /// <summary>
  /// Default constructor
  /// </summary>
  public Centroid() { }

  /// <summary>
  /// Initializes <see cref="Centroid"/> with initial <paramref name="x"/> and <paramref name="y"/>
  /// </summary>
  public Centroid(int x, int y) : base(x, y) { }

  /// <summary>
  /// Initializes <see cref="Centroid"/> with initial <paramref name="location"/>
  /// </summary>
  public Centroid(Point location) : base(location) { }

  /// <summary>
  /// Copy constructor
  /// </summary>
  public Centroid(Centroid centroid) : base(centroid) { }

  /// <summary>
  /// Creates a deep copy of this <see cref="Centroid"/> instance.
  /// </summary>
  /// <returns>A new <see cref="Centroid"/> with identical state to this instance.</returns>
  public override CanvasObject Clone() => new Centroid(this);

  /// <summary>
  /// Gets the <see cref="GraphicsPath"/> representing the shape of this <see cref="Centroid"/> (circle and crosshairs).
  /// </summary>
  protected override GraphicsPath Path
  {
    get
    {
      var center = new Point(POINT_DIAMETER / 2, POINT_DIAMETER / 2);
      var topLeftPoint = ToPoint.Subtract(center);
      var circleRect = new Rectangle(topLeftPoint.X, topLeftPoint.Y, POINT_DIAMETER, POINT_DIAMETER);
      var path = new GraphicsPath();
      // Add circle
      path.AddEllipse(circleRect);
      // Add crosshairs
      int cx = ToPoint.X;
      int cy = ToPoint.Y;
      int r = POINT_DIAMETER / 2;
      // Horizontal line
      path.StartFigure();
      path.AddLine(cx - r, cy, cx + r, cy);
      // Vertical line
      path.StartFigure();
      path.AddLine(cx, cy - r, cx, cy + r);
      return path;
    }
  }

  /// <summary>
  /// Draws the centroid as a transparent outlined circle with cross hairs.
  /// </summary>
  public override void Draw(Graphics graphics)
  {
    if (!Visible) return;

    // Use a distinct color and thickness if selected
    Color outlineColor = IsSelected ? Color.OrangeRed : OutlineColor;
    float outlineWidth = IsSelected ? 2f : 2f;
    Color crosshairColor = IsSelected ? Color.OrangeRed : CrosshairColor;
    float crosshairWidth = IsSelected ? 2f : 1f;

    using (var pen = new Pen(outlineColor, outlineWidth))
    {
      graphics.DrawPath(pen, Path);
    }
  }
}
