﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using TNT.Drawing.Objects;

namespace TNT.Drawing.Layers
{
	/// <summary>
	/// Represents a layer managed by the <see cref="Canvas"/>
	/// </summary>
	[Serializable]
	public class CanvasLayer
	{
		/// <summary>
		/// Backing fields for properties
		/// </summary>
		protected BackingFields BackingFields = new BackingFields();

		/// <summary>
		/// Reference the <see cref="Canvas"/>
		/// </summary>
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
		public List<CanvasObject> CanvasObjects { get; set; } = new List<CanvasObject>();

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
		public CanvasLayer() : this(null)
		{

		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public CanvasLayer(Canvas canvas)
		{
			Canvas = canvas;
			BackingFields.OnFieldChanged = (_, __) => IsInvalid = true;
		}

		/// <summary>
		/// Gets the selected <see cref="CanvasObject"/>
		/// </summary>
		/// <returns>Selected <see cref="CanvasObject"/></returns>
		public virtual List<CanvasObject> GetSelected() => CanvasObjects.FindAll(o => o.IsSelected);

		/// <summary>
		/// Override to return the <see cref="Name"/>
		/// </summary>
		/// <returns><see cref="Name"/></returns>
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

		/// <summary>
		/// Sets <see cref="IsInvalid"/> to true
		/// </summary>
		public void Invalidate() => IsInvalid = true;
	}
}
