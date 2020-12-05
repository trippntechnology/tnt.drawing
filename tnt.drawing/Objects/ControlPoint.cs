using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects
{
	public class ControlPoint : CanvasPoint
	{
		public override Image Image => Resources.Images.ControlPoint;

		private Pen pen = new Pen(Color.FromArgb(100, Color.Black));

		public override bool Visible => LinkedPoints.FirstOrDefault(p => p.Id != Id && p.Equals(this)) == null;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ControlPoint() { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public ControlPoint(ControlPoint controlPoint) : base(controlPoint) { }

		public ControlPoint(Vertex vertex) : base(vertex)
		{
			AddLinkedPoints(vertex);
			vertex.AddLinkedPoints(this);
		}

		public override void Draw(Graphics graphics)
		{
			if (!Visible) return;
			LinkedPoints.FirstOrDefault(p => p is Vertex).Let(p => graphics.DrawLine(pen, ToPoint, p.ToPoint));
			base.Draw(graphics);
		}

		public override void MoveBy(int dx, int dy, Keys modifierKeys)
		{
			base.MoveBy(dx, dy, modifierKeys);

			if (modifierKeys == Keys.Shift)
			{
				// Get vertex point
				var vertex = LinkedPoints.FirstOrDefault(p => p is Vertex);
				// Get the opposite control point
				var opposite = vertex?.LinkedPoints.FirstOrDefault(p => p.Id != Id);

				var offset = vertex.ToPoint.Subtract(ToPoint);
				var newPoint = vertex.ToPoint.Add(offset);
				opposite?.MoveTo(newPoint);
			}
		}

		public override CanvasObject Copy()
		{
			throw new NotImplementedException();
		}
	}
}
