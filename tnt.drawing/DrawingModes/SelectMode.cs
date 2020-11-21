using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes
{
	public class SelectMode : DrawingMode
	{
		private Point previousMouseLocation = Point.Empty;
		private List<CanvasObject> selectedObjects = new List<CanvasObject>();

		public override CanvasObject DefaultObject => null;

		public override CanvasLayer Layer => Canvas?.Layers?.Find(l => string.Equals(l.Name, "Object")) ?? new CanvasLayer();

		public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
		{
			var foundObject = FindObjectAt(Layer.CanvasObjects, e.Location, modifierKeys);
			foundObject?.OnMouseDown(e, modifierKeys);

			if (foundObject == null)
			{
				selectedObjects.Clear();
			}
			else if (modifierKeys == Keys.Shift && selectedObjects.Contains(foundObject))
			{
				selectedObjects.Remove(foundObject);
			}
			else if (modifierKeys == Keys.Shift && !selectedObjects.Contains(foundObject))
			{
				selectedObjects.Add(foundObject);
			}
			else if (!selectedObjects.Contains(foundObject))
			{
				selectedObjects.Clear();
				selectedObjects.Add(foundObject);

			}

			Canvas.OnObjectsSelected(selectedObjects.Select(o => o as object).ToList());

			// Select/unselect objects
			Layer.CanvasObjects.ForEach(o => o.IsSelected = selectedObjects.Contains(o));

			base.OnMouseDown(e, modifierKeys);
		}

		public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseUp(e, modifierKeys);
		}

		public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
		{
			Debug.WriteLine($"OnMouseMove\nmodifierKeys: {modifierKeys}");
			var dx = e.X - previousMouseLocation.X;
			var dy = e.Y - previousMouseLocation.Y;
			previousMouseLocation = e.Location;

			CanvasObject objUnderMouse = FindObjectAt(Layer.CanvasObjects, e.Location, modifierKeys);
			Log($"dx: {dx} dy: {dy} e.X: {e.X} e.Y: {e.Y} Location: {e.Location} objUnderMouse: {objUnderMouse}");

			if (IsMouseDown) selectedObjects.ForEach(o => o.MoveBy(dx, dy, modifierKeys));

			Canvas.Cursor = objUnderMouse == null ? Cursors.Default : Cursors.Hand;
			Refresh(Layer);
			base.OnMouseMove(e, modifierKeys);
		}

		protected CanvasObject FindObjectAt(List<CanvasObject> objs, Point mouseLocation, Keys modifierKeys) => objs.FirstOrDefault(o => o.MouseOver(mouseLocation, modifierKeys) != null);
	}
}
