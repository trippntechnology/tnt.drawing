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
	public class Square : CanvasObject
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public Brush SolidBrush { get; set; }

		public Square(int x, int y, int width, Color color) : base()
		{
			X = x;
			Y = y;
			Width = width;
			SolidBrush = new SolidBrush(color);
		}

		public override void Draw(Graphics graphics)
		{
			graphics.FillRectangle(SolidBrush, X, Y, Width, Width);
		}

		public override CanvasObject Copy()
		{
			throw new NotImplementedException();
		}

		public override CanvasObject MouseOver(Point mousePosition, Keys modifierKeys)
		{
			var path = new GraphicsPath();
			path.AddRectangle(new Rectangle(X, Y, Width, Width));
			return path.IsVisible(mousePosition) ? this : null;
		}
	}
}
