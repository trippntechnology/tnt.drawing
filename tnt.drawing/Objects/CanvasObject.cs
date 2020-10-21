using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

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
		public string Id { get; set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		protected CanvasObject() { Id = Guid.NewGuid().ToString(); }

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
		/// Implement by subclass to indicate mouse is over object
		/// </summary>
		public abstract bool MouseOver(Point mousePosition, Keys modifierKeys);

		/// <summary>
		/// Gets an image associated with the <paramref name="resource"/> value within the calling assembly
		/// </summary>
		protected Image ResourceToImage(string resource)
		{
			var assembly = Assembly.GetCallingAssembly();
			var resourceStream = assembly.GetManifestResourceStream(resource);
			return resourceStream == null ? null : new Bitmap(resourceStream);
		}
	}
}
