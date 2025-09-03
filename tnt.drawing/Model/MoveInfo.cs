using System.Drawing;

namespace TNT.Drawing.Model;

/// <summary>
/// Contains information about a mouse move event, including the current location and the delta from the previous location.
/// </summary>
/// <param name="Location">The current mouse location.</param>
/// <param name="Delta">The difference from the previous mouse location.</param>
public record MoveInfo(Point Location, int Dx, int Dy)
{
  public MoveInfo(Point location) : this(location, 0, 0) { }

  public static MoveInfo Create(Point Location) => new MoveInfo(Location);
}
