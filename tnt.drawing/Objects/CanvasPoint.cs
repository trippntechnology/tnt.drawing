using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TNT.Drawing.Extensions;
using TNT.Drawing.Model;

namespace TNT.Drawing.Objects;

/// <summary>
/// Represents a <see cref="Point"/> on the <see cref="Canvas"/>
/// </summary>
public class CanvasPoint() : CanvasObject
{
  protected const int POINT_DIAMETER = 10;

  /// <summary>
  /// <see cref="Action{CanvasPoint, Int, Int, Keys}"/> delegate that is called when the <see cref="CanvasPoint"/>
  /// moves
  /// </summary>
  public Action<CanvasPoint, int, int, Keys> OnMoved = (canvasPoint, dx, dy, modifierKeys) => { };

  /// <summary>
  /// X coordinate
  /// </summary>
  public int X { get => Get(0); set => Set(value); }

  /// <summary>
  /// Y coordinate
  /// </summary>
  public int Y { get => Get(0); set => Set(value); }

  /// <summary>
  /// Converts a <see cref="CanvasPoint"/> to a <see cref="Point"/>
  /// </summary>
  public virtual Point ToPoint => new Point(X, Y);

  /// <summary>
  /// Indicates whether the <see cref="CanvasPoint"/> is visible or not
  /// </summary>
  public virtual bool Visible { get => true; }

  /// <summary>
  /// Initializes the <see cref="X"/> and <see cref="Y"/> coordinates
  /// </summary>
  public CanvasPoint(int x, int y) : this() { X = x; Y = y; }

  /// <summary>
  /// Copy constructor
  /// </summary>
  public CanvasPoint(CanvasPoint canvasPoint) : this(canvasPoint.X, canvasPoint.Y) { }

  /// <summary>
  /// Creates a deep copy of this <see cref="CanvasPoint"/> instance.
  /// </summary>
  /// <returns>A new <see cref="CanvasPoint"/> with identical state to this instance.</returns>
  public override CanvasObject Clone() => new CanvasPoint(this);

  /// <summary>
  /// Draws the <see cref="CanvasPoint"/>
  /// </summary>
  public override void Draw(Graphics graphics)
  {
    if (!Visible) return;
    var center = new Point(POINT_DIAMETER / 2, POINT_DIAMETER / 2);
    var topLeftPoint = ToPoint.Subtract(center);

    if (IsSelected)
      graphics.FillEllipse(new SolidBrush(Color.FromArgb(128, Color.Black)), topLeftPoint.X, topLeftPoint.Y, POINT_DIAMETER, POINT_DIAMETER);
    else
      graphics.DrawEllipse(new Pen(Color.Black, 1), topLeftPoint.X, topLeftPoint.Y, POINT_DIAMETER, POINT_DIAMETER);
  }

  /// <summary>
  /// Moves this <see cref="CanvasPoint"/> by <paramref name="dx"/> and <paramref name="dy"/>
  /// </summary>
  public override void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false) => MoveTo(new Point(X + dx, Y + dy), modifierKeys, supressCallback);

  /// <summary>
  /// Moves this <see cref="CanvasPoint"/> to the location specified by <paramref name="point"/>
  /// </summary>
  public virtual void MoveTo(Point point, Keys modifierKeys, bool supressCallback = false)
  {
    var delta = point.Subtract(ToPoint);
    X = point.X;
    Y = point.Y;

    if (!supressCallback)
    {
      OnMoved(this, delta.X, delta.Y, modifierKeys);
    }
  }

  /// <summary>
  /// Call to check if the mouse is over this <see cref="CanvasPoint"/>
  /// </summary>
  /// <returns><see cref="CanvasPoint"/> when mouse is over the this</returns>
  public override MouseOverResponse MouseOver(Point mousePosition, Keys modifierKeys)
  {
    var topLeftPoint = ToPoint.Subtract(new Point(POINT_DIAMETER / 2, POINT_DIAMETER / 2));
    var path = new GraphicsPath();
    path.AddEllipse(topLeftPoint.X, topLeftPoint.Y, POINT_DIAMETER, POINT_DIAMETER);
    return path.IsVisible(mousePosition) ? new MouseOverResponse(this) : MouseOverResponse.Default;
  }

  /// <summary>
  /// Aligns this <see cref="CanvasPoint"/> with the <paramref name="alignInterval"/>
  /// </summary>
  public override void Align(int alignInterval)
  {
    var point = new Point(X, Y).Snap(alignInterval);
    (X, Y) = point.Deconstruct();
  }

  /// <summary>
  /// Determines whether the specified object is equal to the current <see cref="CanvasPoint"/>.
  /// Two <see cref="CanvasPoint"/> instances are considered equal if their base <see cref="CanvasObject"/> is equal and their X and Y coordinates are equal.
  /// </summary>
  /// <param name="obj">The object to compare with the current object.</param>
  /// <returns><c>true</c> if the specified object is a <see cref="CanvasPoint"/> with the same Id, X, and Y; otherwise, <c>false</c>.</returns>
  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(this, obj)) return true;
    if (obj is not CanvasPoint other) return false;
    if (!base.Equals(obj)) return false;
    return X == other.X && Y == other.Y;
  }

  /// <summary>
  /// Returns a hash code for this <see cref="CanvasPoint"/> based on its <see cref="Id"/>, <see cref="X"/>, and <see cref="Y"/> properties.
  /// </summary>
  /// <returns>A hash code for the current object.</returns>
  public override int GetHashCode() => HashCode.Combine(Id, X, Y);

  override public string ToString() => $"{base.ToString()} [{X}, {Y}]";

  /// <summary>
  /// Determines whether this <see cref="CanvasPoint"/> is within a specified distance of another <see cref="CanvasPoint"/>.
  /// </summary>
  /// <param name="other">The other <see cref="CanvasPoint"/> to compare against.</param>
  /// <param name="threshold">The maximum distance (in pixels) to consider the points as "near"; defaults to <see cref="POINT_DIAMETER"/>.</param>
  /// <returns><c>true</c> if the distance between the points is less than or equal to <paramref name="threshold"/>; otherwise, <c>false</c>.</returns>
  public bool IsNear(CanvasPoint other, int threshold = POINT_DIAMETER)
  {
    int dx = X - other.X;
    int dy = Y - other.Y;
    int distanceSquared = dx * dx + dy * dy;
    return distanceSquared <= threshold * threshold;
  }
}
