using System;
using System.Xml.Serialization;

namespace TNT.Drawing.Objects
{
	public class ControlPoint : CanvasPoint
	{
		public string EndpointId { get; set; }

		[XmlIgnore()]
		public Vertex Endpoint { get; set; }

		public override bool Visible
		{
			get => !Endpoint?.ToPoint.Equals(ToPoint) == true;
			set => base.Visible = value;
		}

		public ControlPoint() { }

		public ControlPoint(CanvasPoint controlPoint) : base(controlPoint) { }

		public ControlPoint(Vertex endPoint) : base(endPoint.X, endPoint.Y)
		{
			EndpointId = endPoint.Id;
			Endpoint = endPoint;
			Endpoint.OnPositionChanged = (x, y, dx, dy) => { X = x; Y = y; };
		}

		public override CanvasObject Copy()
		{
			throw new NotImplementedException();
		}
	}
}
