using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml.Serialization;
using TNT.Commons;
using TNT.Drawing.Extensions;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects;

/// <summary>
/// Represents a <see cref="Point"/> on the <see cref="Canvas"/>
/// </summary>
public class CanvasPoint : CanvasObject
{
  /// <summary>
  /// <see cref="Image"/> that represents this point
  /// </summary>
  public virtual Image Image => Resources.Images.Vertex;

  /// <summary>
  /// <see cref="Action{CanvasPoint, Int, Int, Keys}"/> delegate that is called when the <see cref="CanvasPoint"/>
  /// moves
  /// </summary>
  [XmlIgnore]
  public Action<CanvasPoint, int, int, Keys> OnPointMoved = (canvasPoint, dx, dy, modifierKeys) => { };

  /// <summary>
  /// X coordinate
  /// </summary>
  public int X { get; set; }

  /// <summary>
  /// Y coordinate
  /// </summary>
  public int Y { get; set; }

  /// <summary>
  /// Converts a <see cref="CanvasPoint"/> to a <see cref="Point"/>
  /// </summary>
  public virtual Point ToPoint => new Point(X, Y);

  /// <summary>
  /// Indicates whether the <see cref="CanvasPoint"/> is visible or not
  /// </summary>
  [XmlIgnore]
  public virtual bool Visible { get => true; }

  /// <summary>
  /// Default constructor
  /// </summary>
  public CanvasPoint() { }

  /// <summary>
  /// Initializes the <see cref="X"/> and <see cref="Y"/> coordinates
  /// </summary>
  public CanvasPoint(int x, int y) { X = x; Y = y; }

  /// <summary>
  /// Copy constructor
  /// </summary>
  public CanvasPoint(CanvasPoint controlPoint) : this(controlPoint.X, controlPoint.Y) { }

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
    Image?.Let(image =>
    {
      var imageCenter = new Point(Image.Width / 2, Image.Height / 2);
      var topLeftPoint = ToPoint.Subtract(imageCenter);
      graphics.DrawImage(Image, topLeftPoint);
      return 0;
    });
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
      OnPointMoved(this, delta.X, delta.Y, modifierKeys);
    }
  }

  /// <summary>
  /// Call to check if the mouse is over this <see cref="CanvasPoint"/>
  /// </summary>
  /// <returns><see cref="CanvasPoint"/> when mouse is over the this</returns>
  public override CanvasObject? MouseOver(Point mousePosition, Keys modifierKeys)
  {
    if (Image == null) return null;
    var width = Image.Width;
    var height = Image.Height;

    var topLeftPoint = ToPoint.Subtract(new Point(width / 2, height / 2));
    var path = new GraphicsPath();
    path.AddEllipse(topLeftPoint.X, topLeftPoint.Y, width, height);
    return path.IsVisible(mousePosition) ? this : null;
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
