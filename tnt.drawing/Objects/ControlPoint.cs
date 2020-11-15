using System;

namespace TNT.Drawing.Objects
{
	public class ControlPoint : CanvasPoint
	{
		public override bool Visible
		{
			get => true;
			set => base.Visible = value;
		}

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ControlPoint() { }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public ControlPoint(CanvasPoint controlPoint) : base(controlPoint) { }

		public override CanvasObject Copy()
		{
			throw new NotImplementedException();
		}
	}
}
