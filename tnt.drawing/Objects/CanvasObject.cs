using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TNT.Drawing.Objects
{
	/// <summary>
	/// Base class for all <see cref="Canvas"/> objects
	/// </summary>
	public abstract class CanvasObject
	{
		/// <summary>
		/// <see cref="Guid"/> of the object
		/// </summary>
		[ReadOnly(true)]
		public string Id { get; set; }

		[XmlIgnore]
		public bool IsSelected { get; set; } = false;

		/// <summary>
		/// Default constructor
		/// </summary>
		protected CanvasObject() => Id = Guid.NewGuid().ToString();

		/// <summary>
		/// Copy constructor
		/// </summary>
		protected CanvasObject(CanvasObject canvasObject) => Id = canvasObject.Id;

		/// <summary>
		/// Implemented by subclass to draw object
		/// </summary>
		public abstract void Draw(Graphics graphics);

		/// <summary>
		/// Implement by subclass to copy object
		/// </summary>
		public abstract CanvasObject Copy();

		/// <summary>
		/// Implement by subclass to indicate mouse is over object. Should return the object that is under mouse.
		/// </summary>
		public abstract CanvasObject MouseOver(Point mousePosition, Keys modifierKeys);

		public abstract void MoveBy(int dx, int dy, Keys modifierKeys);

		public virtual CanvasObject OnMouseDown(Point location, Keys modifierKeys) => null;

		public virtual Cursor GetCursor(Point location, Keys keys) => Cursors.Default;
	}
}
