using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNT.Drawing.Objects;

namespace TNT.Drawing.Layer
{
	public class CanvasLayer
	{
		public bool Visible { get; set; } = true;

		public List<CanvasObject> CanvasObjects { get; set; }

		public virtual void Draw(Graphics graphics) => CanvasObjects?.ForEach(o => o.Draw(graphics));
	}
}
