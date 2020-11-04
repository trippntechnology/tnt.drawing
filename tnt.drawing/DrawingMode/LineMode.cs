using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingMode
{
	public class LineMode : DrawingMode
	{
		Line ActiveLine = null;
		Vertex ActiveVertex = null;

		public override CanvasObject DefaultObject { get; } = new Line();

		public override void OnKeyDown(Graphics graphics, KeyEventArgs e)
		{
			base.OnKeyDown(graphics, e);
		}

		public override void OnKeyUp(Graphics graphics, KeyEventArgs e)
		{
			base.OnKeyUp(graphics, e);
		}

		public override void OnMouseClick(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			Debug.WriteLine($"LineMode.OnMouseClick({e.Location})");
			base.OnMouseClick(graphics, e, modifierKeys);

			if (ActiveLine == null)
			{
				var line1 = new Line();

				ActiveLine = DefaultObject.Copy() as Line;
				ActiveLine.AddVertex(new Vertex(e.X, e.Y));
				ActiveVertex = new Vertex(e.X, e.Y);
				ActiveLine.AddVertex(ActiveVertex);
				RequestLayer().CanvasObjects.Add(ActiveLine);
				RefreshCanvas();
			}
			else if (ActiveVertex != null)
			{
				ActiveVertex = new Vertex(e.X, e.Y);
				ActiveLine.AddVertex(ActiveVertex);
				RefreshCanvas();
			}
		}

		public override void OnMouseDoubleClick(Graphics graphics, MouseEventArgs e)
		{
			base.OnMouseDoubleClick(graphics, e);

			// Remove last three
			var points = ActiveLine.Points;
			points.RemoveRange(points.Count - 3, 3);

			ActiveVertex = null;
			ActiveLine = null;
			RefreshCanvas();
		}

		public override void OnMouseDown(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseDown(graphics, e, modifierKeys);
		}

		public override void OnMouseMove(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			Debug.WriteLine($"LineMode.OnMouseMove({e.Location})");
			base.OnMouseMove(graphics, e, modifierKeys);


			//if (ActiveVertex != null)
			//{
			//	ActiveVertex.X = e.X;
			//	ActiveVertex.Y = e.Y;
			//	RefreshCanvas();
			//}

			Canvas.Refresh();
			graphics.DrawRectangle(new Pen(Color.Blue), e.Location.X, e.Location.Y, 4, 4);
		}

		public override void OnMouseUp(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseUp(graphics, e, modifierKeys);
		}

		protected void RunNotNull<A, B>(A arg1, B arg2, Action<A, B> callback)
		{
			if (arg1 != null && arg2 != null) callback(arg1, arg2);
		}
	}
}
