using System.Drawing;

namespace TNT.Drawing.Objects;

/// <summary>
/// Represents a vertex <see cref="CanvasPoint"/>
/// </summary>
public class Vertex : CanvasPoint
{
  /// <summary>
  /// Default constructor
  /// </summary>
  public Vertex() { }

  /// <summary>
  /// Initializes <see cref="Vertex"/> with initial <paramref name="x"/> and <paramref name="y"/>
  /// </summary>
  public Vertex(int x, int y) : base(x, y) { }

  /// <summary>
  /// Initializes <see cref="Vertex"/> with initial <paramref name="location"/>
  /// </summary>
  public Vertex(Point location) : base(location.X, location.Y) { }

  /// <summary>
  /// Copy constructor
  /// </summary>
  public Vertex(Vertex vertex) : base(vertex) { }

  /// <summary>
  /// Creates a deep copy of this <see cref="Vertex"/> instance.
  /// </summary>
  /// <returns>A new <see cref="Vertex"/> with identical state to this instance.</returns>
  public override CanvasObject Clone() => new Vertex(this);


}