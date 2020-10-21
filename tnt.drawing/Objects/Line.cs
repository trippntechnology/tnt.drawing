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
		Pen Pen;
		List<Vertex> Vertices = new List<Vertex>();

		public Line(List<Vertex> vertices = null, Pen pen = null)
		{
			Vertices = vertices ?? new List<Vertex>();
			Pen = pen ?? new Pen(Color.Blue);
		}

		public Line(Line line) : this(line.Vertices, line.Pen) { }

		public void AddVertex(Vertex vertex)
		{
			Vertices.Add(vertex);
		}

		public override CanvasObject Copy() => new Line(this);

		public override bool MouseOver(Point mousePosition, Keys modifierKeys)
		{
			var path = new GraphicsPath();
			var points = Vertices.Select(v => v.ToPoint).ToArray();
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
			path.StartFigure();
			var points = Vertices.Select(v => v.ToPoint).ToArray();

			if (points.Count() == 2)
			{
				path.AddLines(points);
			}
			else
			{
				path.AddPolygon(points);
			}

			path.CloseFigure();

			graphics.DrawPath(Pen, path);

			Vertices.ForEach(v => v.Draw(graphics));
		}
	}
}
