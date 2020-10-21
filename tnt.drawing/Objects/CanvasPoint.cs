using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TNT.Drawing.Objects
{
	public class CanvasPoint : CanvasObject
	{
		private Image _Image = null;

		public Image Image
		{
			get
			{
				if (_Image == null) _Image = ResourceToImage(ImageResource);
				return _Image;
			}
		}

		public int X { get; set; }
		public int Y { get; set; }

		protected virtual string ImageResource => "TNT.Drawing.Image.ControlPoint.png";

		public virtual Point ToPoint => new Point(X, Y);

		public CanvasPoint() { }

		public CanvasPoint(int x, int y) { X = x; Y = y; }

		public CanvasPoint(CanvasPoint controlPoint) : this(controlPoint.X, controlPoint.Y) { }

		public override CanvasObject Copy() => new CanvasPoint(this);

		public override void Draw(Graphics graphics)
		{
			var imageCenter = new Point(Image.Width / 2, Image.Height / 2);
			var topLeftPoint = ToPoint.Subtract(imageCenter);
			graphics.DrawImage(Image, topLeftPoint);
		}

		public override bool MouseOver(Point mousePosition, Keys modifierKeys)
		{
			throw new NotImplementedException();
		}
	}
}
