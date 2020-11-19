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
				Debug.WriteLine($"Set Points");
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

				p1.AddLinkedPoints(c1);
				c1.AddLinkedPoints(p1);
				vertex.AddLinkedPoints(c2);
				c2.AddLinkedPoints(vertex);

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

		public override CanvasObject Copy() => new Line(this);

		public override CanvasObject MouseOver(Point mousePosition, Keys modifierKeys)
		{
			CanvasObject objUnderMouse = null;

			if (IsSelected)
			{
				// Check points
				if (modifierKeys == Keys.Control)
				{
					// Check ControlPoints points
					objUnderMouse = Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(mousePosition, modifierKeys) != null);
				}
				else
				{
					// Check visible points
					objUnderMouse = Points.FirstOrDefault(p => p.Visible && p.MouseOver(mousePosition, modifierKeys) != null);
				}
			}
			else if (Points.Count > 3)
			{
				// Check line
				var path = new GraphicsPath();
				var points = Points.Select(v => v.ToPoint).ToArray();
				path.AddBeziers(points);
				objUnderMouse = path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) ? this : null;
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

		public override void MoveBy(int dx, int dy, Keys modifierKeys) => Points.FindAll(p => p is Vertex)?.ForEach(p => p.MoveBy(dx, dy, modifierKeys));
	}
}