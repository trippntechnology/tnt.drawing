using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TNT.Drawing.Objects
{
	public class ControlPoint : CanvasPoint
	{
		public string EndpointId { get; set; }

		[XmlIgnore()]
		public Vertex EndPoint { get; set; }

		public ControlPoint() { }

		public ControlPoint(CanvasPoint controlPoint) : base(controlPoint) { }

		public ControlPoint(Vertex endPoint) : base(endPoint.X, endPoint.Y)
		{
			EndpointId = endPoint.Id;
			EndPoint = endPoint;
			EndPoint.OnPositionChanged = (x, y, dx, dy) =>
			{
				X = x;
				Y = y;
			};
		}

		public override CanvasObject Copy()
		{
			throw new NotImplementedException();
		}
	}
}
