using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TNT.Drawing.Objects
{
	public class Line : CanvasObject
	{
		public List<CanvasPoint> Points = new List<CanvasPoint>();

		public Pen Pen { get; set; } = new Pen(Color.Black);

		public Line() : base() { }

		public Line(Line line) : this() { Pen = line.Pen; }

		public void AddVertex(Vertex vertex)
		{
			if (Points.Count >= 4) Points.Add(new ControlPoint(Points.Last()));
			Points.Add(vertex);
			Points.Add(new ControlPoint(vertex));
		}

		public override CanvasObject Copy() => new Line(this);

		public override bool MouseOver(Point mousePosition, Keys modifierKeys)
		{
			var path = new GraphicsPath();
			var points = Points.Select(v => v.ToPoint).ToArray();
			path.AddLines(points);

			return path.IsVisible(mousePosition);
		}

		public override void Draw(Graphics graphics)
		{
			//for (var index = 1; index < Vertices.Count; index++)
			//{
			//	var p1 = Vertices[index - 1].Location;
			//	var p2 = Vertices[index].Location;
			//	graphics.DrawLine(Pen, p1, p2);
			//}

			var path = new GraphicsPath();
			//path.StartFigure();
			var points = Points.Select(v => v.ToPoint).ToArray();
			if (points.Length < 4) return;

			//if (points.Count() == 2)
			//{
			//	path.AddLines(points);
			//}
			//else
			//{
			path.AddBeziers(points);
			//	path.AddPolygon(points);
			//}

			//path.CloseFigure();

			graphics.DrawPath(Pen, path);

			Points.ForEach(v => v.Draw(graphics));
		}
	}
}
