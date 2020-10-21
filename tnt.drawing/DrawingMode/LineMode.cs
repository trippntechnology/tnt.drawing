using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingMode
{
	public class LineMode : DrawingMode
	{
		Line DefaultLine = new Line();

		Line ActiveLine = null;
		Vertex ActiveVertex = null;

		public override DrawingMode Copy() => new LineMode();

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
				ActiveLine = (Line)DefaultLine.Copy();
				ActiveLine.AddVertex(new Vertex(e.X, e.Y));
				ActiveVertex = new Vertex(e.X, e.Y);
				ActiveLine.AddVertex(ActiveVertex);
				AddObject(ActiveLine);
				RequestRefresh();
				//graphics.DrawLine(new Pen(Color.Blue), (Point)start, (Point)end);
			}
			else if (ActiveVertex != null)
			{
				ActiveVertex = new Vertex(e.X, e.Y);
				ActiveLine.AddVertex(ActiveVertex);
				RequestRefresh();
			}
		}

		public override void OnMouseDoubleClick(Graphics graphics, MouseEventArgs e)
		{
			base.OnMouseDoubleClick(graphics, e);

			ActiveVertex = null;
			ActiveLine = null;
			RequestRefresh();
		}

		public override void OnMouseDown(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseDown(graphics, e, modifierKeys);
		}

		public override void OnMouseMove(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			Debug.WriteLine($"LineMode.OnMouseMove({e.Location})");
			base.OnMouseMove(graphics, e, modifierKeys);

			if (ActiveVertex != null)
			{
				ActiveVertex.X = e.X;
				ActiveVertex.Y = e.Y;
				RequestRefresh();
			}
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
