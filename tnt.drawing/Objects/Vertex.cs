using System.Drawing;

namespace TNT.Drawing.Objects
{
	public class Vertex : CanvasPoint
	{
		protected override string ImageResource => "TNT.Drawing.Image.Vertex.png";

		public Vertex() { }

		public Vertex(int x, int y) : base(x, y) { }

		public Vertex(Vertex vertex) : base(vertex) { }

		public override CanvasObject Copy() => new Vertex(this);

		public override void MoveBy(int dx, int dy)
		{
			LinkedPoints.ForEach(p => p.MoveBy(dx, dy));
			base.MoveBy(dx, dy);
		}

		public void MoveTo(Point point)
		{
			var dx = point.X - X;
			var dy = point.Y - Y;
			LinkedPoints.ForEach(p => p.MoveBy(dx, dy));
			base.MoveTo(point);
		}
	}
}