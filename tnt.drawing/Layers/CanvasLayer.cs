using System;
using System.Collections.Generic;
using System.Drawing;
using TNT.Drawing.Objects;
using TNT.Reactive;

namespace TNT.Drawing.Layers;

/// <summary>
/// Represents a layer managed by the <see cref="Canvas"/>
/// </summary>
[Serializable]
public class CanvasLayer
{
  /// <summary>  
  /// The `_Redraw` field is a protected boolean flag used to indicate whether the  
  /// `CanvasLayer` needs to be redrawn.  
  ///  
  /// - It is initialized to `true` by default.  
  /// - The `_Redraw` flag is set to `true` whenever a property change is detected  
  ///   via the `_BackingFields.OnFieldChanged` event.  
  /// - This mechanism ensures that the layer is marked for redraw when its state  
  ///   changes, such as updates to dimensions, visibility, or background color.  
  /// </summary>  
  protected bool _Redraw = true;

  /// <summary>
  /// Backing fields for properties
  /// </summary>
  protected BackingFields _BackingFields = new BackingFields();

  /// <summary>
  /// Reference the <see cref="Canvas"/>
  /// </summary>
  public Canvas? Canvas;

  /// <summary>
  /// Indicates whether the <see cref="CanvasLayer"/> is visisble
  /// </summary>
  public bool IsVisible { get; set; } = true;

  /// <summary>
  /// Background color 
  /// </summary>
  public Color BackgroundColor { get => _BackingFields.Get<Color>(Color.Transparent); set => _BackingFields.Set(value); }

  /// <summary>
  /// The width of the <see cref="CanvasLayer"/>
  /// </summary>
  public int Width { get => _BackingFields.Get(1024); set => _BackingFields.Set(value); }

  /// <summary>
  /// The height of the <see cref="CanvasLayer"/>
  /// </summary>
  public int Height { get => _BackingFields.Get(768); set => _BackingFields.Set(value); }

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
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Default constructor for <see cref="CanvasLayer"/>.
  /// Subscribes to property change notifications from <see cref="BackingFields"/> and sets the redraw flag when any property changes.
  /// </summary>
  public CanvasLayer()
  {
    _BackingFields.OnFieldChanged += (field, value) => { _Redraw = true; };
  }

  /// <summary>
  /// Initialization constructor
  /// </summary>
  public CanvasLayer(Canvas canvas) : this()
  {
    Canvas = canvas;
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
    if (IsVisible)
    {
      // Draw background
      graphics.FillRectangle(new SolidBrush(BackgroundColor), Rect);

      // Draw objects
      CanvasObjects?.ForEach(o => { if (!o.IsSelected) o.Draw(graphics); });
    }
  }
}
