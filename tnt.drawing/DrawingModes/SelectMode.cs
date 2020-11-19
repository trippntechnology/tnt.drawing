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
		private Point _PreviousMouseLocation = Point.Empty;
		private CanvasObject _SelectedObject = null;

		public override CanvasObject DefaultObject => null;

		public override CanvasLayer Layer => Canvas?.Layers?.Find(l => string.Equals(l.Name, "Object")) ?? new CanvasLayer();

		public override void OnMouseClick(MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseClick(e, modifierKeys);

			var objs = Layer.CanvasObjects;
			CanvasObject underMouse = GetObjectUnderMouse(objs, e.Location, modifierKeys);
			Canvas.OnObjectsSelected(underMouse != null ? new List<object>() { underMouse } : null);
			Canvas.Invalidate(Layer);
		}

		public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
		{
			_SelectedObject = GetObjectUnderMouse(Layer.CanvasObjects, e.Location, modifierKeys);
			Debug.WriteLine($"selectedObject: {_SelectedObject}");
			if (_SelectedObject == null) Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
			if (_SelectedObject != null) _SelectedObject.IsSelected = true;

			if (modifierKeys == (Keys.Shift | Keys.Control))
			{
				_SelectedObject.Delete();
			}
			base.OnMouseDown(e, modifierKeys);
		}

		public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
		{
			_SelectedObject = null;
			base.OnMouseUp(e, modifierKeys);
		}

		public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
		{
			Debug.WriteLine($"OnMouseMove\nmodifierKeys: {modifierKeys}");
			var dx = e.X - _PreviousMouseLocation.X;
			var dy = e.Y - _PreviousMouseLocation.Y;
			_PreviousMouseLocation = e.Location;

			CanvasObject objUnderMouse = GetObjectUnderMouse(Layer.CanvasObjects, e.Location, modifierKeys);
			Log($"dx: {dx} dy: {dy} e.X: {e.X} e.Y: {e.Y} Location: {e.Location} objUnderMouse: {objUnderMouse}");

			_SelectedObject?.MoveBy(dx, dy, modifierKeys);

			Canvas.Cursor = objUnderMouse == null ? Cursors.Default : Cursors.Hand;
			Refresh(Layer);
			base.OnMouseMove(e, modifierKeys);
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
	}
}
