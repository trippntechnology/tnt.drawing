using System.Drawing;
using System.Drawing.Drawing2D;
using TNT.Drawing.Extensions;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects;

/// <summary>
/// Represents a centroid, drawn as an image with a circular hit area matching the image size.
/// </summary>
public class Centroid : CanvasPoint
{
  private static readonly SolidBrush _brush = new(Color.Blue);

  // The image used to represent the centroid visually.
  private Image _image = Resources.Images.Rotate;

  /// <summary>
  /// Initializes a new instance of the <see cref="Centroid"/> class.
  /// </summary>
  public Centroid() { }

  /// <summary>
  /// Initializes a new instance of the <see cref="Centroid"/> class with the specified coordinates.
  /// </summary>
  public Centroid(int x, int y) : base(x, y) { }

  /// <summary>
  /// Initializes a new instance of the <see cref="Centroid"/> class by copying another <see cref="Centroid"/>.
  /// </summary>
  public Centroid(Centroid centroid) : base(centroid) { }

  /// <summary>
  /// Creates a deep copy of this <see cref="Centroid"/> instance.
  /// </summary>
  /// <returns>A new <see cref="Centroid"/> with identical state to this instance.</returns>
  public override CanvasObject Clone() => new Centroid(this);

  /// <summary>
  /// Gets the <see cref="GraphicsPath"/> representing a circle matching the image's width, centered at <see cref="ToPoint"/>.
  /// </summary>
  protected override GraphicsPath Path
  {
    get
    {
      int diameter = _image.Width;
      var center = ToPoint;
      var topLeft = new Point(center.X - diameter / 2, center.Y - diameter / 2);
      var circleRect = new Rectangle(topLeft.X, topLeft.Y, diameter, diameter);
      var path = new GraphicsPath();
      path.AddEllipse(circleRect);
      return path;
    }
  }

  /// <summary>
  /// Draws the centroid as an image. If selected, applies a tint; otherwise, draws the image normally.
  /// </summary>
  public override void Draw(Graphics graphics)
  {
    if (!Visible) return;

    int size = _image.Width;
    int x = ToPoint.X - size / 2;
    int y = ToPoint.Y - size / 2;

    if (IsSelected)
    {
      // Draw the image at the centroid location with a blue tint (for demonstration)
      graphics.DrawImage(_image, ToPoint, Color.Blue);
    }
    else
    {
      // Draw the image at the centroid location without tint
      graphics.DrawImage(_image, new Rectangle(x, y, size, size));
    }
  }
}
