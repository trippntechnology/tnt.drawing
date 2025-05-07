using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TNT.Drawing.Model;

namespace TNT.Drawing.Objects
{
  public class Square : CanvasObject
  {
    private SolidBrush _SolidBrush = new SolidBrush(Color.Black);

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Color Color { get; set; } = Color.Black;

    public Square() { }

    public Square(int x, int y, int width, Color color) : base()
    {
      X = x;
      Y = y;
      Width = width;
      Height = width;
      Color = color;
    }

    public override void Draw(Graphics graphics)
    {
      _SolidBrush.Color = this.Color;
      graphics.FillRectangle(_SolidBrush, X, Y, Width, Height);
    }

    public override CanvasObject Copy()
    {
      throw new NotImplementedException();
    }

    public override MouseOverResponse MouseOver(Point mousePosition, Keys modifierKeys)
    {
      var path = new GraphicsPath();
      path.AddRectangle(new Rectangle(X, Y, Width, Width));
      return path.IsVisible(mousePosition) ? new MouseOverResponse(this) : MouseOverResponse.Default;
    }

    public override void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false)
    {
      X += dx;
      Y += dy;
    }

    public override void Align(int alignInterval)
    {
    }
  }
}
