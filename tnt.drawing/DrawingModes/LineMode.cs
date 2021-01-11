using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes
{
	public class LineMode : DrawingMode
	{
		private Line ActiveLine = null;
		private Vertex ActiveVertex = null;
		private CanvasPoint Marker = null;

		public override CanvasObject DefaultObject { get; } = new Line();

		public LineMode(CanvasLayer layer) : base(layer) { }

		public override void Reset()
		{
			ActiveLine = null;
			ActiveVertex = null;
			Marker = null;
			base.Reset();
		}

		public override void OnMouseClick(MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseClick(e, modifierKeys);
			var location = Canvas.SnapToInterval ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

			if (e.Button == MouseButtons.Left)
			{
				if (ActiveLine == null)
				{
					var line1 = new Line();

					ActiveLine = DefaultObject.Copy() as Line;
					ActiveLine.IsSelected = true;
					ActiveLine.AddVertex(new Vertex(location));
					ActiveVertex = new Vertex(location);
					ActiveLine.AddVertex(ActiveVertex);
					Refresh();
				}
				else if (ActiveVertex != null)
				{
					ActiveVertex = new Vertex(location);
					ActiveLine.AddVertex(ActiveVertex);
					Refresh();
				}
			}
			else if (e.Button == MouseButtons.Right && ActiveVertex != null && ActiveLine != null)
			{
				ActiveLine.RemoveVertex(ActiveVertex);
				ActiveVertex = ActiveLine.PointsArray.Last() as Vertex;
				Debug.WriteLine($"ActiveVertex: {ActiveVertex}");
				if (ActiveLine.PointsArray.Length < 2)
				{
					ActiveLine = null;
					ActiveVertex = null;
				}
				Refresh();
			}
		}

		public override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			if (ActiveLine != null)
			{
				// Remove last vertex
				ActiveLine.RemoveVertex(ActiveVertex);
				ActiveLine.IsSelected = false;
				Layer?.CanvasObjects?.Add(ActiveLine);
				ActiveVertex = null;
				ActiveLine = null;
				Refresh(Layer);
			}
		}

		public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseDown(e, modifierKeys);
		}

		public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseMove(e, modifierKeys);
			var location = Canvas.SnapToInterval?  e.Location.Snap(Canvas.SnapInterval) : e.Location;

			if (ActiveVertex != null)
			{
				ActiveVertex.MoveTo(location, modifierKeys);
				Marker = null;
			}
			else
			{
				if (Marker == null) Marker = new CanvasPoint();
				Marker.MoveTo(location);
			}
			Invalidate();
		}
		public override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Marker?.Draw(e.Graphics);
			ActiveLine?.Draw(e.Graphics);
		}

		public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseUp(e, modifierKeys);
		}
	}
}
