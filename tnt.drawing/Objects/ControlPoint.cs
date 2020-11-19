using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TNT.Drawing.Objects
{
	public class ControlPoint : CanvasPoint
	{
		private Pen pen = new Pen(Color.FromArgb(100, Color.Black));

		public override bool Visible
		{
			get
			{
				var isVisible = LinkedPoints.FirstOrDefault(p => p.Id != Id && p.Equals(this)) == null;
				Debug.WriteLine($"this: {this.Id}  isVisible: {isVisible}");
				return isVisible;
			}
		}

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ControlPoint() { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public ControlPoint(CanvasPoint controlPoint) : base(controlPoint) { }

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

		public override void Delete()
		{
			var vertex = LinkedPoints.FirstOrDefault(p => p is Vertex);
			MoveTo(vertex.ToPoint);
		}

		public override CanvasObject Copy()
		{
			throw new NotImplementedException();
		}
	}
}
