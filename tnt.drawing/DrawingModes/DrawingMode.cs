using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes
{
	/// <summary>
	/// Base class for all drawing modes
	/// </summary>
	public class DrawingMode
	{
		/// <summary>
		/// Indicates when the mouse button is pressed
		/// </summary>
		protected bool IsMouseDown = false;

		/// <summary>
		/// Reference to the <see cref="Canvas"/>
		/// </summary>
		public Canvas Canvas { get; set; }

		/// <summary>
		/// The <see cref="CanvasLayer"/> manipulated by this <see cref="DrawingMode"/>
		/// </summary>
		public CanvasLayer Layer { get; protected set; }

		/// <summary>
		/// The <see cref="CanvasObject"/> that gets created by the <see cref="DrawingMode"/>
		/// </summary>
		public virtual CanvasObject DefaultObject { get; } = null;

		/// <summary>
		/// Initialization constructor
		/// </summary>
		public DrawingMode(CanvasLayer layer) { this.Layer = layer; }

		/// <summary>
		/// Refreshes the <see cref="Canvas"/> with the current <see cref="CanvasLayer"/>
		/// </summary>
		public virtual void Reset() => Refresh(Layer);

		/// <summary>
		/// Called when the mouse moves across the <see cref="Canvas"/>
		/// </summary>
		public virtual void OnMouseMove(MouseEventArgs e, Keys modifierKeys) => Log();

		/// <summary>
		/// Called when the mouse button is released
		/// </summary>
		public virtual void OnMouseUp(MouseEventArgs e, Keys modifierKeys) { IsMouseDown = false; Log(); }

		/// <summary>
		/// Called when the mouse button is pressed
		/// </summary>
		public virtual void OnMouseDown(MouseEventArgs e, Keys modifierKeys) { IsMouseDown = true; Log(); }

		/// <summary>
		/// Called when the mouse button is double clicked
		/// </summary>
		public virtual void OnMouseDoubleClick(MouseEventArgs e) => Log();

		/// <summary>
		/// Called when the mouse button is clicked
		/// </summary>
		public virtual void OnMouseClick(MouseEventArgs e, Keys modifierKeys) => Log();

		/// <summary>
		/// Called when a key is pressed
		/// </summary>
		public virtual void OnKeyDown(KeyEventArgs e) => Log();

		/// <summary>
		/// Called when a key is released
		/// </summary>
		public virtual void OnKeyUp(KeyEventArgs e) => Log();

		/// <summary>
		/// Called when the <see cref="Canvas"/> is being painted
		/// </summary>
		public virtual void OnPaint(PaintEventArgs e) => Log();

		/// <summary>
		/// Calls <see cref="Canvas.Refresh(CanvasLayer)"/>
		/// </summary>
		protected virtual void Refresh(CanvasLayer layer = null) => Canvas?.Refresh(layer);

		/// <summary>
		/// Calls <see cref="Canvas.Invalidate(CanvasLayer)"/>
		/// </summary>
		/// <param name="layer"></param>
		protected virtual void Invalidate(CanvasLayer layer = null) => Canvas?.Invalidate(layer);

		/// <summary>
		/// Called to log to <see cref="Debug"/>
		/// </summary>
		protected virtual void Log(string msg = "", [CallerMemberName] string callingMethod = null) => Debug.WriteLine($"{DateTime.Now} [{callingMethod}] {msg}");
	}
}
