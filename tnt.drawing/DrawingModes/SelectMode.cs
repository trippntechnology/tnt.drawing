using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes
{
	public class SelectMode : DrawingMode
	{
		public override CanvasObject DefaultObject => null;

		public override CanvasLayer Layer => Canvas?.Layers?.Find(l => string.Equals(l.Name, "Object")) ?? new CanvasLayer();


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
			base.OnMouseClick(graphics, e, modifierKeys);

			var objs = Layer.CanvasObjects;
			objs.ForEach(o => o.IsSelected = false);
			CanvasObject underMouse = GetObjectUnderMouse(objs, e.Location, modifierKeys);
			if (underMouse != null) underMouse.IsSelected = true;
			Canvas.OnObjectsSelected(underMouse != null ? new List<object>() { underMouse } : null);
			Canvas.Invalidate(Layer);
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
			CanvasObject underMouse = GetObjectUnderMouse(Layer.CanvasObjects, e.Location, modifierKeys);
			Canvas.Cursor = underMouse == null ? Cursors.Default : Cursors.Hand;
			base.OnMouseMove(graphics, e, modifierKeys);
		}

		protected CanvasObject GetObjectUnderMouse(List<CanvasObject> objs, Point mouseLocation, Keys modifierKeys)
		{
			CanvasObject objUnderMouse = null;
			for (var index = 0; objUnderMouse == null && index < objs.Count; index++)
			{
				objUnderMouse = objs[index].MouseOver(mouseLocation, modifierKeys);
			}
			return objUnderMouse;
		}

		public override void OnMouseUp(Graphics graphics, MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseUp(graphics, e, modifierKeys);
		}
	}
}
