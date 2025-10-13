using System.Drawing;

namespace TNT.Drawing.Model;

/// <summary>
/// Represents information about a mouse move event, including the current mouse location and the change in X and Y coordinates (delta) since the previous event.
/// </summary>
public record MoveInfo(Point MouseLocation, int Dx, int Dy)
{
  public Point PreviousLocation => new Point(MouseLocation.X - Dx, MouseLocation.Y - Dy);

  public MoveInfo(Point mouseLocation) : this(mouseLocation, 0, 0) { }

  public static MoveInfo Create(Point mouseLocation) => new MoveInfo(mouseLocation);
}
