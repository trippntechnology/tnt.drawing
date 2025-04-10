using System;
using System.Drawing;
using System.Xml.Serialization;
using TNT.Drawing.Extensions;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects
{
	/// <summary>
	/// Represents a controll point on the <see cref="Canvas"/>
	/// </summary>
	public class ControlPoint : CanvasPoint
	{
		/// <summary>
		/// <see cref="Func{ControlPoint, Boolean}"/> delegate called to see if this <see cref="ControlPoint"/>
		/// should be visible or not
		/// </summary>
		public Func<ControlPoint, bool> IsVisible { get; set; } = (_) => { return false; };

		/// <summary>
		/// Indicates whether the <see cref="ControlPoint"/> is visible or not
		/// </summary>
		[XmlIgnore]
		public override bool Visible => IsVisible(this);

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ControlPoint() { }

		/// <summary>
		/// Initializes the <see cref="ControlPoint"/> with an initial <paramref name="initPoint"/>
		/// </summary>
		/// <param name="initPoint"></param>
		public ControlPoint(Point initPoint) : base(initPoint.X, initPoint.Y) { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public ControlPoint(ControlPoint controlPoint) : base(controlPoint) { }

		/// <summary>
		/// Draws the <see cref="ControlPoint"/> if <see cref="Visible"/>
		/// </summary>
		public override void Draw(Graphics graphics)
		{
			if (!Visible) return;
      var center = new Point(POINT_DIAMETER / 2, POINT_DIAMETER / 2);
      var topLeftPoint = ToPoint.Subtract(center);
			var bottomRightPoint = ToPoint.Add(center);
      graphics.DrawRectangle(new Pen(Color.Black, 1), topLeftPoint.X, topLeftPoint.Y, POINT_DIAMETER, POINT_DIAMETER);
    }

    /// <summary>
    /// Copies this <see cref="ControlPoint"/>
    /// </summary>
    /// <returns>Copy of this <see cref="ControlPoint"/></returns>
    public override CanvasObject Copy() => new ControlPoint(this);
	}
}
