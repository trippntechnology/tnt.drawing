using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using TNT.Drawing.Objects;

namespace TNT.Drawing.Layers
{
	[Serializable]
	public class CanvasLayer
	{
		protected BackingFields BackingFields = new BackingFields();
		protected Canvas Canvas = null;

		/// <summary>
		/// Indicates whether <see cref="Draw(Graphics)"/> should redraw the <see cref="Image"/>
		/// </summary>
		protected bool IsInvalid = true;

		/// <summary>
		/// The <see cref="Bitmap"/> drawn by the layer
		/// </summary>
		protected Bitmap Image;


		/// <summary>
		/// Delegate that is called when the <see cref="GridLayer"/> needs to be refreshed. 
		/// </summary>
		[XmlIgnore]
		public Action OnRefreshRequest = () => { };

		/// <summary>
		/// Indicates whether the <see cref="CanvasLayer"/> is visisble
		/// </summary>
		public bool IsVisible { get; set; } = true;

		/// <summary>
		/// Background color 
		/// </summary>
		public Color BackgroundColor { get => BackingFields.Get<Color>(Color.Transparent); set => BackingFields.Set(value); }

		/// <summary>
		/// The width of the <see cref="CanvasLayer"/>
		/// </summary>
		public int Width { get => BackingFields.Get(1024); set => BackingFields.Set(value); }

		/// <summary>
		/// The height of the <see cref="CanvasLayer"/>
		/// </summary>
		public int Height { get => BackingFields.Get(768); set => BackingFields.Set(value); }

		/// <summary>
		/// <see cref="CanvasObject"/> managed by this layer
		/// </summary>
		public List<CanvasObject> CanvasObjects { get; set; }

		/// <summary>
		/// A <see cref="Rectangle"/> that represents the area of this <see cref="CanvasLayer"/>
		/// </summary>
		public Rectangle Rect => new Rectangle(0, 0, Width, Height);

		/// <summary>
		/// Name of Layer
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// Default constructor
		/// </summary>
		public CanvasLayer(Canvas canvas)
		{
			Canvas = canvas;
			BackingFields.OnFieldChanged = (_, __) => IsInvalid = true;
		}


		public override string ToString() => Name;

		/// <summary>
		/// Draws the layer
		/// </summary>
		public virtual void Draw(Graphics graphics)
		{
			// Draw background
			graphics.FillRectangle(new SolidBrush(BackgroundColor), Rect);

			// Draw objects
			CanvasObjects?.ForEach(o => { if (!o.IsSelected) o.Draw(graphics); });
		}

		public void Invalidate() => IsInvalid = true;
	}
}
