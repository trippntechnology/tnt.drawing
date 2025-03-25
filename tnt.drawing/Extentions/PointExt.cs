using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TNT.Drawing.Extentions;

public static class PointExt
{
  /// <summary>
  /// Transforms a <see cref="Point"/> from <see cref="CoordinateSpace.Page"/> to <see cref="CoordinateSpace.World"/>
  /// with the current <paramref name="graphics"/>
  /// </summary>
  public static Point ToGridCoordinates(this Point value, Graphics graphics)
  {
    var points = new Point[] { value };
    graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Page, points);
    return points[0];
  }

  /// <summary>
  /// Transforms a <see cref="Point"/> from <see cref="CoordinateSpace.World"/> to <see cref="CoordinateSpace.Page"/>
  /// with the current <paramref name="graphics"/>
  /// </summary>
  public static Point ToCanvasCoordinates(this Point value, Graphics graphics)
  {
    var points = new Point[] { value };
    graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, points);
    return points[0];
  }

  /// <summary>
  /// Subtracts <see cref="Point"/> p2 from <see cref="Point"/> p1
  /// </summary>
  public static Point Subtract(this Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);

  /// <summary>
  /// Adds <see cref="Point"/> p2 to <see cref="Point"/> p1
  /// </summary>
  public static Point Add(this Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);

  /// <summary>
  /// Returns a new <see cref="Point"/> that is adjusted to the <paramref name="snapInterval"/>
  /// </summary>
  /// <returns>A new <see cref="Point"/> that is adjusted to the <paramref name="snapInterval"/></returns>
  public static Point Snap(this Point point, int snapInterval)
  {
    var modX = point.X % snapInterval;
    var modY = point.Y % snapInterval;
    var newX = point.X / snapInterval * snapInterval;
    var newY = point.Y / snapInterval * snapInterval;
    point.X = modX >= snapInterval / 2 ? newX + snapInterval : newX;
    point.Y = modY >= snapInterval / 2 ? newY + snapInterval : newY;
    return point;
  }

  /// <summary>
  /// Deconstructs the <see cref="Point"/> into its X and Y components
  /// </summary>
  /// <returns>Deconstructed components of a <see cref="Point"/></returns>
  public static (int, int) Deconstruct(this Point point) => (point.X, point.Y);

  /// <summary>
  /// Adds <paramref name="value"/> to <paramref name="list"/> if <paramref name="value"/> is not null
  /// </summary>
  public static void AddNotNull<T>(this List<T> list, T value)
  {
    if (value != null) list.Add(value);
  }
}
