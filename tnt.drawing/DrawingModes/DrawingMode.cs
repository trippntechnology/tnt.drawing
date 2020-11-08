using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes
{
	public abstract class DrawingMode
	{
		public Canvas Canvas { get; set; }

		//public Action OnRequestRefresh = () => { };
		//public Func<CanvasLayer> OnLayerRequest = () => { return null; };
		//public Action<List<object>> OnObjectsSelected = (_) => { };
		public abstract CanvasLayer Layer { get; }

		public abstract CanvasObject DefaultObject { get; }

		public virtual void Reset() => Canvas?.Refresh(Layer);

		public virtual void OnMouseMove(Graphics graphics, MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnMouseUp(Graphics graphics, MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnMouseDown(Graphics graphics, MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnMouseDoubleClick(Graphics graphics, MouseEventArgs e) => Log();
		public virtual void OnMouseClick(Graphics graphics, MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnKeyDown(Graphics graphics, KeyEventArgs e) => Log();
		public virtual void OnKeyUp(Graphics graphics, KeyEventArgs e) => Log();
		public virtual void OnPaint(PaintEventArgs e) => Log();

		protected virtual void Refresh(CanvasLayer layer = null) => Canvas?.Refresh(layer);
		protected virtual void Invalidate(CanvasLayer layer = null) => Canvas?.Invalidate(layer);

		protected virtual CanvasLayer RequestLayer() => Canvas?.Layers.LastOrDefault();
		protected virtual void ObjectsSelected(List<object> objs) => Canvas?.OnSelected(objs);

		protected virtual void Log(string msg = "", [CallerMemberName] string callingMethod = null) => Debug.WriteLine($"{DateTime.Now} [{callingMethod}] {msg}");
	}
}
