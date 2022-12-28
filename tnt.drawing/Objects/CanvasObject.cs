using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using TNT.Drawing.Resource;

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

		/// <summary>
		/// Indicates whether the object is selected or not
		/// </summary>
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
		/// Implement to align the object at the given <paramref name="alignInterval"/>
		/// </summary>
		public abstract void Align(int alignInterval);

		/// <summary>
		/// Implement by subclass to indicate mouse is over object. Should return the object that is under mouse.
		/// </summary>
		public abstract CanvasObject? MouseOver(Point mousePosition, Keys modifierKeys);

		/// <summary>
		/// Implement by subclass to move the object by <paramref name="dx"/> and <paramref name="dy"/>
		/// </summary>
		public abstract void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false);

		/// <summary>
		/// Called when a button press event occurs over an object
		/// </summary>
		/// <returns><see cref="CanvasObject"/> under mouse at the time of the button press</returns>
		public virtual CanvasObject OnMouseDown(Point location, Keys modifierKeys, out bool allowMove)
		{
			allowMove = true;
			return null;
		}

		/// <summary>
		/// Called when a button press event occurs over an object
		/// </summary>
		/// <returns><see cref="CanvasObject"/> under mouse at the time of the button press</returns>
		public virtual CanvasObject? OnMouseUp(Point location, Keys modifierKeys) => null;

		/// <summary>
		/// Implement to return a <see cref="Feedback"/> indicating the <see cref="Cursor"/> and hint
		/// to display
		/// </summary>
		/// <returns><see cref="Feedback"/> indicating the <see cref="Cursor"/> and hint to display</returns>
		public virtual Feedback GetFeedback(Point location, Keys keys) => new Feedback(Cursors.Default, string.Empty);
	}
}
