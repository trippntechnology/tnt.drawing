using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TNT.Drawing.Objects
{
	public class Vertex : CanvasPoint
	{
		protected override string ImageResource => "TNT.Drawing.Image.Vertex.png";

		public Vertex() { }

		public Vertex(int x, int y) : base(x, y) { }

		public Vertex(Vertex vertex) : base(vertex) { }

		public override CanvasObject Copy() => new Vertex(this);

		public override bool MouseOver(Point mousePosition, Keys modifierKeys)
		{
			throw new NotImplementedException();
		}
	}
}
