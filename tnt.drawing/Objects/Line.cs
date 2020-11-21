using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TNT.Drawing.Objects
{
	public class Line : CanvasObject
	{
		private CanvasObject activeObject = null;
		private Pen _Pen = new Pen(Color.Black, 1);

		public float Width { get; set; } = 1;

		public int ColorARGB { get => this.Color.ToArgb(); set => this.Color = Color.FromArgb(value); }

		[XmlIgnore]
		public Color Color { get; set; } = Color.Blue;

		public DashStyle Style { get; set; } = DashStyle.Solid;

		[XmlIgnore]
		protected List<CanvasPoint> Points { get; set; } = new List<CanvasPoint>();

		/// <summary>
		/// Needed for deserialization so that set gets called. 
		/// </summary>
		public CanvasPoint[] PointsArray
		{
			get => Points.ToArray();
			set
			{
				Points = value.ToList();
				var ctrlPoints = Points.ToList().FindAll(p => p is ControlPoint);
				var vertices = Points.ToList().FindAll(p => p is Vertex);
				ctrlPoints.ForEach(p =>
				{
					var linkedPoints = p.LinkedPointIds.Select(s => vertices.Find(v => v.Id == s));
					p.AddLinkedPoints(linkedPoints.ToArray());
				});
			}
		}

		public Pen Pen
		{
			get
			{
				_Pen.Color = Color;
				_Pen.Width = Width;
				_Pen.DashStyle = Style;
				return _Pen;
			}
		}

		public Line() : base() { }

		public Line(Line line) : this() { Width = line.Width; Color = line.Color; }

		public void AddVertex(Vertex vertex)
		{
			vertex.Parent = this;

			if (Points.Count > 0)
			{
				var p1 = Points.Last(p => p is Vertex) as Vertex;
				var c1 = new ControlPoint(p1) { Parent = this };
				var c2 = new ControlPoint(vertex) { Parent = this };

				Points.Add(c1);
				Points.Add(c2);
			}
			Points.Add(vertex);
		}

		public void RemoveVertex(Vertex vertex)
		{
			vertex.LinkedPointIds.ForEach(id => Points.RemoveAll(p => p.Id == id));
			Points.Remove(vertex);
			var lastPoint = Points.LastOrDefault();
			if (lastPoint is ControlPoint)
			{
				lastPoint.LinkedPoints.ForEach(p => p.RemovedLinkedPoints(lastPoint));
				Points.Remove(lastPoint);
			}
		}

		public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
		{
			activeObject = this;

			if (IsSelected)
			{
				CanvasPoint point = null;

				// Check points
				if (modifierKeys == (Keys.Shift | Keys.Control))
				{
					// Find point
					point = Points.FirstOrDefault(p => p is Vertex && p.MouseOver(e.Location, modifierKeys) != null) ??
						Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(e.Location, modifierKeys) != null);
					point?.Delete();
				}
				else if (modifierKeys == Keys.Control)
				{
					// Check ControlPoints points
					point = Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(e.Location, modifierKeys) != null);
				}
				else
				{
					// Check visible points
					point = Points.FirstOrDefault(p => p.Visible && p.MouseOver(e.Location, modifierKeys) != null);
				}

				if (point == null && modifierKeys == Keys.Control)
				{
					var path = new GraphicsPath();
					var pen = new Pen(Color.Black, 10);
					int? insertIndex = null;

					// Find the line that the mouse is over
					for (var index = 3; insertIndex == null && index < Points.Count(); index += 4)
					{
						var points = Points.GetRange(index - 3, 4).Select(p => p.ToPoint);
						path.ClearMarkers();
						path.AddBeziers(points.ToArray());

						if (path.IsOutlineVisible(e.Location, pen)) insertIndex = index - 1;
					}

					if (insertIndex != null)
					{
						var vertex = new Vertex(e.X, e.Y) { Parent = this };
						var c1 = new ControlPoint(vertex) { Parent = this };
						var c2 = new ControlPoint(vertex) { Parent = this };
						Points.InsertRange((int)insertIndex, new List<CanvasPoint>() { c1, vertex, c2 });
					}
				}

				activeObject = point ?? activeObject;
			}
		}

		public override CanvasObject Copy() => new Line(this);

		public override CanvasObject MouseOver(Point mousePosition, Keys modifierKeys)
		{
			CanvasObject objUnderMouse = null;

			if (Points.Count > 3)
			{
				// Check if over this line
				var path = new GraphicsPath();
				var points = Points.Select(v => v.ToPoint).ToArray();
				path.AddBeziers(points);
				objUnderMouse = path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) ? this : null;

				if (objUnderMouse == null)
				{
					// Check if over any points
					objUnderMouse = Points.FirstOrDefault(p => p.MouseOver(mousePosition, modifierKeys) != null) != null ? this : null;
				}
			}

			return objUnderMouse;
		}

		public override void Draw(Graphics graphics)
		{
			var path = new GraphicsPath();
			var points = Points.Select(v => v.ToPoint).ToArray();
			if (points.Length < 4) return;
			path.AddBeziers(points);
			graphics.DrawPath(Pen, path);

			if (IsSelected) Points.ForEach(v => v.Draw(graphics));
		}

		public override void MoveBy(int dx, int dy, Keys modifierKeys)
		{
			if (activeObject == this)
			{
				Points.FindAll(p => p is Vertex)?.ForEach(p => p.MoveBy(dx, dy, modifierKeys));
			}
			else
			{
				activeObject?.MoveBy(dx, dy, modifierKeys);
			}
		}
	}
}