using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TNT.Drawing.Objects;

namespace TNT.Drawing.Layers
{
	public class CanvasLayer
	{
		protected BackingFields BackingFields = new BackingFields();

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
		public CanvasLayer() { BackingFields.OnFieldChanged = (_, __) => IsInvalid = true; }


		public override string ToString() => Name;

		/// <summary>
		/// Draws the layer
		/// </summary>
		public virtual void Draw(Graphics graphics)
		{
			if (!IsVisible) return;
			if (IsInvalid)
			{
				Image = new Bitmap(Width, Height);
				var imageGraphics = Graphics.FromImage(Image);
				DrawImage(imageGraphics);
			}

			graphics.DrawImage(Image, Rect);
		}

		public void Invalidate() => IsInvalid = true;

		/// <summary>
		/// 
		/// </summary>
		protected virtual void DrawImage(Graphics graphics)
		{
			graphics.FillRectangle(new SolidBrush(BackgroundColor), Rect);
			CanvasObjects?.ForEach(o => o.Draw(graphics));
			IsInvalid = false;
		}
	}
}
