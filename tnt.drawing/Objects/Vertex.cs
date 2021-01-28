using System.Drawing;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects
{
	/// <summary>
	/// Represents a vertex <see cref="CanvasPoint"/>
	/// </summary>
	public class Vertex : CanvasPoint
	{
		/// <summary>
		/// <see cref="Image"/> that represents this point
		/// </summary>
		public override Image Image => Resources.Images.Vertex;

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
		/// Copies this <see cref="Vertex"/>
		/// </summary>
		/// <returns>Copy of this <see cref="Vertex"/></returns>
		public override CanvasObject Copy() => new Vertex(this);
	}
}