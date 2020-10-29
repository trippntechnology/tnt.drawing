using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingMode
{
	public class SelectMode : DrawingMode
	{
		public override CanvasObject DefaultObject => null;

		public override void OnKeyDown(Graphics graphics, KeyEventArgs e)
		{
			base.OnKeyDown(graphics, e);
		}

		public override void OnKeyUp(Graphics graphics, KeyEventArgs e)
		{
			base.OnKeyUp(graphics, e);
		}

		public override void OnMouseClick(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			//Debug.WriteLine($"SelectMode.OnMouseClick({e.Location})");
			base.OnMouseClick(graphics, e, modifierKeys);

			var objs = RequestLayer().CanvasObjects;
			CanvasObject underMouse = null;
			for (var index = 0; underMouse == null && index < objs.Count; index++)
			{
				underMouse = objs[index].MouseOver(e.Location, modifierKeys);
			}
			OnObjectsSelected(new List<CanvasObject>() { underMouse });
		}

		public override void OnMouseDoubleClick(Graphics graphics, MouseEventArgs e)
		{
			base.OnMouseDoubleClick(graphics, e);
		}

		public override void OnMouseDown(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseDown(graphics, e, modifierKeys);
		}

		public override void OnMouseMove(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			var objs = RequestLayer().CanvasObjects;
			CanvasObject underMouse = null;
			for(var index = 0; underMouse == null && index < objs.Count; index++)
			{
				underMouse = objs[index].MouseOver(e.Location, modifierKeys);
			}

			//Debug.WriteLine($"SelectMode.OnMouseMove({e.Location})");
			Debug.WriteLine($"SelectMode.OnMouseMove({underMouse})");

			base.OnMouseMove(graphics, e, modifierKeys);
		}

		public override void OnMouseUp(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseUp(graphics, e, modifierKeys);
		}
	}
}
