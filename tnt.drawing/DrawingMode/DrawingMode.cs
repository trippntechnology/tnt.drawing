using System;
using System.Drawing;
using System.Windows.Forms;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingMode
{
	public abstract class DrawingMode
	{
		public Action<CanvasObject> OnAddObject = (_) => { };
		public Action OnRequestRefresh = () => { };

		abstract public DrawingMode Copy();

		virtual public void OnMouseMove(Graphics graphics, MouseEventArgs e, Keys modifierKeys) { }
		virtual public void OnMouseUp(Graphics graphics, MouseEventArgs e, Keys modifierKeys) { }
		virtual public void OnMouseDown(Graphics graphics, MouseEventArgs e, Keys modifierKeys) { }
		virtual public void OnMouseDoubleClick(Graphics graphics, MouseEventArgs e) { }
		virtual public void OnMouseClick(Graphics graphics, MouseEventArgs e, Keys modifierKeys) { }
		virtual public void OnKeyDown(Graphics graphics, KeyEventArgs e) { }
		virtual public void OnKeyUp(Graphics graphics, KeyEventArgs e) { }

		virtual protected void AddObject(CanvasObject canvasObject) => OnAddObject(canvasObject);
		virtual protected void RequestRefresh() => OnRequestRefresh();
	}
}
