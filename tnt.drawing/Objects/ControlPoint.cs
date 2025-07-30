using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TNT.Drawing.Extensions;

namespace TNT.Drawing.Objects
{
  /// <summary>
  /// Represents a controll point on the <see cref="Canvas"/>
  /// </summary>
  public class ControlPoint : CanvasPoint
  {
    /// <summary>
    /// <see cref="Func{ControlPoint, Boolean}"/> delegate called to see if this <see cref="ControlPoint"/>
    /// should be visible or not
    /// </summary>
    public Func<ControlPoint, bool> IsVisible { get; set; } = (_) => { return false; };

    /// <summary>
    /// Indicates whether the <see cref="ControlPoint"/> is visible or not
    /// </summary>
    public override bool Visible => IsVisible(this);

    /// <summary>
    /// Default Constructor
    /// </summary>
    public ControlPoint() { }

    /// <summary>
    /// Initializes the <see cref="ControlPoint"/> with an initial <paramref name="initPoint"/>
    /// </summary>
    /// <param name="initPoint"></param>
    public ControlPoint(Point initPoint) : base(initPoint.X, initPoint.Y) { }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public ControlPoint(ControlPoint controlPoint) : base(controlPoint) { }

    /// <summary>
    /// Gets the <see cref="GraphicsPath"/> representing the shape of this <see cref="ControlPoint"/>.
    /// Returns a rectangle path.
    /// </summary>
    protected override GraphicsPath Path
    {
      get
      {
        var center = new Point(POINT_DIAMETER / 2, POINT_DIAMETER / 2);
        var topLeftPoint = ToPoint.Subtract(center);
        var path = new GraphicsPath();
        path.AddRectangle(new Rectangle(topLeftPoint.X, topLeftPoint.Y, POINT_DIAMETER, POINT_DIAMETER));
        return path;
      }
    }

    /// <summary>
    /// Draws the <see cref="ControlPoint"/> if <see cref="Visible"/>
    /// </summary>
    public override void Draw(Graphics graphics)
    {
      if (!Visible) return;
      graphics.DrawPath(OUTLINE_PEN, Path);
    }

    /// <summary>
    /// Creates a deep copy of this <see cref="ControlPoint"/>, including its current state and properties.
    /// </summary>
    /// <returns>Copy of this <see cref="ControlPoint"/></returns>
    public override CanvasObject Clone() => new ControlPoint(this);
  }
}
