using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace TNT.Drawing.Objects
{
	public class Line : CanvasObject
	{
		private Pen Pen = new Pen(Color.Black, 1);

		public List<CanvasPoint> Points = new List<CanvasPoint>();

		public float Width { get => Pen.Width; set => Pen.Width = value; }
		public Color Color { get => Pen.Color; set => Pen.Color = value; }

		public Line() : base() { }

		public Line(Line line) : this() { Width = line.Width; Color = line.Color; }

		public void AddVertex(Vertex vertex)
		{
			//if (Points.Count >= 4) Points.Add(new ControlPoint(Points.Last()));
			if (Points.Count > 0)
			{
				Points.Add(new ControlPoint(Points.Last()));
				Points.Add(new ControlPoint(vertex));
			}
			Points.Add(vertex);
			//Points.Add(new ControlPoint(vertex));
		}

		public override CanvasObject Copy() => new Line(this);

		public override CanvasObject MouseOver(Point mousePosition, Keys modifierKeys)
		{
			var path = new GraphicsPath();
			var points = Points.Select(v => v.ToPoint).ToArray();
			path.AddBeziers(points);
			var stringPoints = points.Select(p => p.ToString()).ToList();
			Debug.WriteLine($"{string.Join(",", stringPoints)}");

			return path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) ? this : null;
		}

		public override void Draw(Graphics graphics)
		{
			var path = new GraphicsPath();
			var points = Points.Select(v => v.ToPoint).ToArray();
			if (points.Length < 4) return;
			path.AddBeziers(points);
			graphics.DrawPath(Pen, path);

			Points.ForEach(v => v.Draw(graphics));
		}
	}
}
