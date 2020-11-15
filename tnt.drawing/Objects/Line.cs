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
		public List<CanvasPoint> Points { get; set; } = new List<CanvasPoint>();

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
			if (Points.Count > 0)
			{
				var p1 = Points.Last(p => p is Vertex) as Vertex;
				var c1 = new ControlPoint(p1);
				var c2 = new ControlPoint(vertex);

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
			var path = new GraphicsPath();
			var points = Points.Select(v => v.ToPoint).ToArray();

			if (points.Length < 4) return null;
			path.AddBeziers(points);
			var stringPoints = points.Select(p => p.ToString()).ToList();
			Debug.WriteLine($"{string.Join(",", stringPoints)}");

			CanvasObject objUnderMouse = null;

			if (IsSelected && modifierKeys == Keys.Control)
			{
				// Check if over control point
				var controlPoints = Points.FindAll(p => p is ControlPoint);
				objUnderMouse = controlPoints.FirstOrDefault(p => p.MouseOver(mousePosition, modifierKeys) != null);
			}
			else if (IsSelected)
			{
				// Check if I'm over vertex
				var vertices = Points.FindAll(p => p is Vertex);
				objUnderMouse = vertices.FirstOrDefault(v => v.MouseOver(mousePosition, modifierKeys) != null);
			}

			return objUnderMouse ?? (path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) ? this : null);
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

		public override void MoveBy(int dx, int dy) => Points.FindAll(p => p is Vertex)?.ForEach(p => p.MoveBy(dx, dy));
	}
}