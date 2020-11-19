using System.Drawing;
using System.Windows.Forms;

namespace TNT.Drawing.Objects
{
	public class Vertex : CanvasPoint
	{
		protected override string ImageResource => "TNT.Drawing.Image.Vertex.png";

		public Vertex() { }

		public Vertex(int x, int y) : base(x, y) { }

		public Vertex(Vertex vertex) : base(vertex) { }

		public override CanvasObject Copy() => new Vertex(this);

		public override void MoveBy(int dx, int dy, Keys modifierKeys)
		{
			LinkedPoints.ForEach(p => p.MoveBy(dx, dy, modifierKeys));
			base.MoveBy(dx, dy, modifierKeys);
		}

		public void MoveTo(Point point, Keys modifierKeys)
		{
			var dx = point.X - X;
			var dy = point.Y - Y;
			LinkedPoints.ForEach(p => p.MoveBy(dx, dy, modifierKeys));
			base.MoveTo(point);
		}

		public override void Delete()
		{
			base.Delete();
			var line = Parent as Line;
			line.RemoveVertex(this);
		}
	}
}