using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Drawing.Layer;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingMode
{
	public class LineMode : DrawingMode
	{
		private Line ActiveLine = null;
		private Vertex ActiveVertex = null;
		private CanvasPoint Marker = null;

		public override CanvasObject DefaultObject { get; } = new Line();

		public override CanvasLayer Layer => Canvas?.Layers?.Find(l => string.Equals(l.Name, "Object"));

		public override void Reset()
		{
			ActiveLine = null;
			ActiveVertex = null;
			Marker = null;
			base.Reset();
		}

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
				Refresh();
			}
			else if (ActiveVertex != null)
			{
				ActiveVertex = new Vertex(e.X, e.Y);
				ActiveLine.AddVertex(ActiveVertex);
				Refresh();
			}
		}

		public override void OnMouseDoubleClick(Graphics graphics, MouseEventArgs e)
		{
			base.OnMouseDoubleClick(graphics, e);

			if (ActiveLine != null)
			{
				// Remove last three
				var points = ActiveLine.Points;
				points.RemoveRange(points.Count - 3, 3);

				Layer?.CanvasObjects?.Add(ActiveLine);

				ActiveVertex = null;
				ActiveLine = null;
				Refresh(Layer);
			}
		}

		public override void OnMouseDown(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseDown(graphics, e, modifierKeys);
		}

		public override void OnMouseMove(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseMove(graphics, e, modifierKeys);

			if (ActiveVertex != null)
			{
				ActiveVertex.X = e.X;
				ActiveVertex.Y = e.Y;
				Marker = null;
			}
			else
			{
				if (Marker == null) Marker = new CanvasPoint();
				Marker.MoveTo(e.Location);
			}
			Invalidate();
		}
		public override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Marker != null) Marker.Draw(e.Graphics);
			ActiveLine?.Draw(e.Graphics);
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
