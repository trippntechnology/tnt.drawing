using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects
{
	public class CanvasPoint : CanvasObject
	{
		public virtual Image Image => Resources.Images.Vertex;

		[XmlIgnore]
		public CanvasObject Parent { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public virtual Point ToPoint => new Point(X, Y);

		[XmlIgnore]
		public virtual bool Visible { get => true; }

		public List<string> LinkedPointIds { get; set; } = new List<string>();
		[XmlIgnore]
		public List<CanvasPoint> LinkedPoints { get; set; } = new List<CanvasPoint>();

		public CanvasPoint() { }

		public CanvasPoint(int x, int y) { X = x; Y = y; }

		public CanvasPoint(CanvasPoint controlPoint) : this(controlPoint.X, controlPoint.Y) { }

		public override CanvasObject Copy() => new CanvasPoint(this);

		public override void Draw(Graphics graphics)
		{
			if (!Visible) return;
			var imageCenter = new Point(Image.Width / 2, Image.Height / 2);
			var topLeftPoint = ToPoint.Subtract(imageCenter);
			graphics.DrawImage(Image, topLeftPoint);
		}

		public override void MoveBy(int dx, int dy, Keys modifierKeys)
		{
			X += dx;
			Y += dy;
		}

		public override CanvasObject MouseOver(Point mousePosition, Keys modifierKeys)
		{
			var imageCenter = new Point(Image.Width / 2, Image.Height / 2);
			var topLeftPoint = ToPoint.Subtract(imageCenter);
			var path = new GraphicsPath();
			path.AddEllipse(topLeftPoint.X, topLeftPoint.Y, Image.Width, Image.Height);
			return path.IsVisible(mousePosition) ? this : null;
		}

		public override void Align(int alignInterval)
		{
			var point = new Point(X, Y).Snap(alignInterval);
			(X, Y) = point.Deconstruct();
		}

		public void MoveTo(Point point)
		{
			X = point.X;
			Y = point.Y;
		}

		public void AddLinkedPoints(params CanvasPoint[] canvasPoints)
		{
			canvasPoints.ToList().ForEach(canvasPoint =>
			{
				if (!LinkedPointIds.Contains(canvasPoint.Id)) LinkedPointIds.Add(canvasPoint.Id);
				if (LinkedPoints.Find(p => p.Id == canvasPoint.Id) == null) LinkedPoints.Add(canvasPoint);
			});
		}

		public void RemovedLinkedPoints(params CanvasPoint[] canvasPoints)
		{
			canvasPoints.ToList().ForEach(CanvasPoint =>
			{
				LinkedPointIds.Remove(CanvasPoint.Id);
				LinkedPoints.Remove(CanvasPoint);
			});
		}

		public override bool Equals(object obj) => obj is CanvasPoint point && Id == point.Id;

		public override int GetHashCode()
		{
			int hashCode = 1861411795;
			hashCode = hashCode * -1521134295 + Id.GetHashCode();
			return hashCode;
		}
	}
}
