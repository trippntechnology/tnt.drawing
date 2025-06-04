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
  protected const int POINT_DIAMETER = 8;

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
  /// Copies this <see cref="CanvasPoint"/>
  /// </summary>
  /// <returns>A copy of this <see cref="CanvasPoint"/></returns>
  public override CanvasObject Copy() => new CanvasPoint(this);

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
  /// Equal operator
  /// </summary>
  /// <returns>True if equal, false if otherwise</returns>
  public override bool Equals(object? obj) => obj is CanvasPoint point && Id == point.Id;

  /// <summary>
  /// Hashcode
  /// </summary>
  /// <returns>Hashcode for this <see cref="CanvasPoint"/></returns>
  public override int GetHashCode()
  {
    int hashCode = 1861411795;
    hashCode = hashCode * -1521134295 + Id.GetHashCode();
    return hashCode;
  }

  override public string ToString() => $"{base.ToString()} [{X}, {Y}]";
}
