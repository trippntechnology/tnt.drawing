using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes
{
	public abstract class DrawingMode
	{
		protected bool IsMouseDown = false;

		public Canvas Canvas { get; set; }

		public abstract CanvasLayer Layer { get; }

		public abstract CanvasObject DefaultObject { get; }

		public virtual void Reset() => Refresh(Layer);

		public virtual void OnMouseMove(MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnMouseUp(MouseEventArgs e, Keys modifierKeys) { IsMouseDown = false; Log(); }
		public virtual void OnMouseDown(MouseEventArgs e, Keys modifierKeys) { IsMouseDown = true; Log(); }
		public virtual void OnMouseDoubleClick(MouseEventArgs e) => Log();
		public virtual void OnMouseClick(MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnKeyDown(KeyEventArgs e) => Log();
		public virtual void OnKeyUp(KeyEventArgs e) => Log();
		public virtual void OnPaint(PaintEventArgs e) => Log();

		protected virtual void Refresh(CanvasLayer layer = null) => Canvas?.Refresh(layer);
		protected virtual void Invalidate(CanvasLayer layer = null) => Canvas?.Invalidate(layer);

		protected virtual void Log(string msg = "", [CallerMemberName] string callingMethod = null) => Debug.WriteLine($"{DateTime.Now} [{callingMethod}] {msg}");
	}
}
