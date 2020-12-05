using System.Drawing;
using System.Windows.Forms;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects
{
	public class Vertex : CanvasPoint
	{
		public override Image Image => Resources.Images.Vertex;

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
	}
}