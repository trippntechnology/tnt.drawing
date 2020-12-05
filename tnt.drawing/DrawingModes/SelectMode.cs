using System.Collections.Generic;
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
		private CanvasObject objectUnderMouse = null;

		public override CanvasObject DefaultObject => null;

		public override CanvasLayer Layer => Canvas?.Layers?.Find(l => string.Equals(l.Name, "Object")) ?? new CanvasLayer();

		public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
		{
			//var objectUnderMouse = FindObjectAt(Layer.CanvasObjects, e.Location, modifierKeys);
			var activeObject = objectUnderMouse?.OnMouseDown(e.Location, modifierKeys);

			if (activeObject != null && objectUnderMouse != activeObject) selectedObjects.Clear();

			if (objectUnderMouse == null)
			{
				selectedObjects.Clear();
			}
			else if (modifierKeys == Keys.Shift && selectedObjects.Contains(objectUnderMouse))
			{
				selectedObjects.Remove(objectUnderMouse);
			}
			else if (modifierKeys == Keys.Shift && !selectedObjects.Contains(objectUnderMouse))
			{
				selectedObjects.Add(objectUnderMouse);
			}
			else if (!selectedObjects.Contains(objectUnderMouse))
			{
				selectedObjects.Clear();
				selectedObjects.Add(objectUnderMouse);
			}

			Canvas.OnObjectsSelected(selectedObjects.Select(o => o as object).ToList());

			// Select/unselect objects
			Layer.CanvasObjects.ForEach(o => o.IsSelected = selectedObjects.Contains(o));

			Refresh(Layer);

			base.OnMouseDown(e, modifierKeys);
		}

		public override void OnKeyDown(KeyEventArgs e)
		{
			Canvas.Cursor = objectUnderMouse?.GetCursor(previousMouseLocation, e.Modifiers) ?? Cursors.Default;
			base.OnKeyDown(e);
		}

		public override void OnKeyUp(KeyEventArgs e)
		{
			Canvas.Cursor = objectUnderMouse?.GetCursor(previousMouseLocation, e.Modifiers) ?? Cursors.Default;
			base.OnKeyUp(e);
		}

		public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
		{
			base.OnMouseUp(e, modifierKeys);
		}

		public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
		{
			//Debug.WriteLine($"OnMouseMove\nmodifierKeys: {modifierKeys}");
			var dx = e.X - previousMouseLocation.X;
			var dy = e.Y - previousMouseLocation.Y;
			previousMouseLocation = e.Location;

			objectUnderMouse = FindObjectAt(Layer.CanvasObjects, e.Location, modifierKeys);
			Log($"dx: {dx} dy: {dy} e.X: {e.X} e.Y: {e.Y} Location: {e.Location} objUnderMouse: {objectUnderMouse}");

			if (IsMouseDown) selectedObjects.ForEach(o => o.MoveBy(dx, dy, modifierKeys));

			Canvas.Cursor = objectUnderMouse?.GetCursor(e.Location, modifierKeys) ?? Cursors.Default;
			Refresh();
			base.OnMouseMove(e, modifierKeys);
		}

		public override void OnPaint(PaintEventArgs e)
		{
			// Draw selected objects
			Layer.CanvasObjects.FindAll(o => o.IsSelected).ForEach(o => o.Draw(e.Graphics));
			base.OnPaint(e);
		}

		protected CanvasObject FindObjectAt(List<CanvasObject> objs, Point mouseLocation, Keys modifierKeys) => objs.FirstOrDefault(o => o.MouseOver(mouseLocation, modifierKeys) != null);
	}
}
