using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using TNT.Drawing.Resource;

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

		protected GraphicsPath Path
		{
			get
			{
				var path = new GraphicsPath();
				var points = Points.Select(v => v.ToPoint).ToArray();
				path.AddBeziers(points);
				return path;
			}
		}

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

		public override CanvasObject OnMouseDown(Point location, Keys modifierKeys)
		{
			activeObject = this;

			if (IsSelected)
			{
				var vertex = Points.FirstOrDefault(p => p is Vertex && p.MouseOver(location, modifierKeys) != null);
				var ctrlPoint = Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, modifierKeys) != null);
				CanvasPoint point = vertex ?? ctrlPoint;

				// Check points
				if (point != null && modifierKeys == (Keys.Shift | Keys.Control))
				{
					// Delete point
					DeletePoint(point);

				}
				else if (ctrlPoint != null && modifierKeys == Keys.Shift)
				{
					// Check ControlPoints points
					point = ctrlPoint;
				}

				if (point == null && modifierKeys == (Keys.Control | Keys.Shift))
				{
					TryAddPoint(location);
				}

				activeObject = point ?? activeObject;
			}

			return activeObject;
		}

		private void TryAddPoint(Point location)
		{
			var path = new GraphicsPath();
			var pen = new Pen(Color.Black, 10);
			int? insertIndex = null;

			// Find the line that the mouse is over
			for (var index = 3; insertIndex == null && index < Points.Count(); index += 3)
			{
				var points = Points.GetRange(index - 3, 4).Select(p => p.ToPoint);
				path.ClearMarkers();
				path.AddBeziers(points.ToArray());

				if (path.IsOutlineVisible(location, pen)) insertIndex = index - 1;
			}

			if (insertIndex != null)
			{
				var vertex = new Vertex(location.X, location.Y) { Parent = this };
				var c1 = new ControlPoint(vertex) { Parent = this };
				var c2 = new ControlPoint(vertex) { Parent = this };
				Points.InsertRange((int)insertIndex, new List<CanvasPoint>() { c1, vertex, c2 });
			}
		}

		private void DeletePoint(CanvasPoint point)
		{
			if (point is Vertex vertex)
			{
				// Only remove if there are two verteces remaining
				if (Points.Sum(p => p is Vertex ? 1 : 0) > 2)
				{
					// Remove the vertex
					RemoveVertex(vertex);
				}
			}
			else if (point is ControlPoint ctrlPoint)
			{
				// Reset position of ControlPoint
				var canvasPoint = ctrlPoint.LinkedPoints.FirstOrDefault(p => p is Vertex);
				ctrlPoint.MoveTo(canvasPoint.ToPoint);
			}
		}

		public override CanvasObject Copy() => new Line(this);

		public override CanvasObject MouseOver(Point mousePosition, Keys modifierKeys)
		{
			CanvasObject objUnderMouse = null;

			if (Points.Count > 3)
			{
				// Check if over this line
				objUnderMouse = Path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) ? this : null;

				if (objUnderMouse == null)
				{
					// Check if over any points that might be outside of the line
					objUnderMouse = Points.FirstOrDefault(p => p.MouseOver(mousePosition, modifierKeys) != null) != null ? this : null;
				}
			}

			return objUnderMouse;
		}

		public override Cursor GetCursor(Point location, Keys keys)
		{
			var cursor = Cursors.Hand;

			// Check if over any points
			if (IsSelected)
			{
				var vertex = Points.FirstOrDefault(p => p is Vertex && p.MouseOver(location, keys) != null);
				var ctrlPoint = Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, keys) != null);
				CanvasPoint point = vertex ?? ctrlPoint;

				if (point != null)
				{
					if (keys == (Keys.Control | Keys.Shift))
					{
						cursor = Resources.Cursors.RemovePoint;
					}
					else if (ctrlPoint != null && keys == Keys.Shift)
					{
						cursor = Resources.Cursors.AddCurve;
					}
					else
					{
						cursor = Resources.Cursors.MovePoint;
					}
				}
				else
				{
					if (keys == (Keys.Control | Keys.Shift))
					{
						cursor = Resources.Cursors.AddPoint;
					}
				}
			}

			return cursor;
		}

		public override void Draw(Graphics graphics)
		{
			graphics.DrawPath(Pen, Path);
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

		public override void Align(int alignInterval) => Points.ForEach(p => p.Align(alignInterval));
	}
}