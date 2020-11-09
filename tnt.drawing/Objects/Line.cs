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
		private Pen _Pen = new Pen(Color.Black, 1);
		public List<CanvasPoint> Points = new List<CanvasPoint>();

		public float Width { get; set; } = 1;
		public Color Color { get; set; } = Color.Blue;
		public DashStyle Style { get; set; } = DashStyle.Solid;

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
				Points.Add(new ControlPoint(Points.Last()));
				Points.Add(new ControlPoint(vertex));
			}
			Points.Add(vertex);
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

			return path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) ? this : null;
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
	}
}
