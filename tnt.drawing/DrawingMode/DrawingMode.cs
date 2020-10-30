using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TNT.Drawing.Layer;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingMode
{
	public abstract class DrawingMode
	{
		//public Action<CanvasObject> OnAddObject = (_) => { };
		public Action OnRequestRefresh = () => { };
		public Func<CanvasLayer> OnLayerRequest = () => { return null; };
		public Action<List<object>> OnObjectsSelected = (_) => { };

		public abstract CanvasObject DefaultObject { get; }

		public virtual void OnMouseMove(Graphics graphics, MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnMouseUp(Graphics graphics, MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnMouseDown(Graphics graphics, MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnMouseDoubleClick(Graphics graphics, MouseEventArgs e) => Log();
		public virtual void OnMouseClick(Graphics graphics, MouseEventArgs e, Keys modifierKeys) => Log();
		public virtual void OnKeyDown(Graphics graphics, KeyEventArgs e) => Log();
		public virtual void OnKeyUp(Graphics graphics, KeyEventArgs e) => Log();

		//protected virtual void AddObject(CanvasObject canvasObject) => OnAddObject(canvasObject);
		protected virtual void RequestRefresh() => OnRequestRefresh();
		protected virtual CanvasLayer RequestLayer() => OnLayerRequest();
		protected virtual void ObjectsSelected(List<object> objs) => OnObjectsSelected(objs);

		protected virtual void Log(string msg = "", [CallerMemberName] string callingMethod = null) => Debug.WriteLine($"{DateTime.Now} [{callingMethod}] {msg}");
	}
}
