using System;
using System.Drawing;
using TNT.Reactive;

namespace TNT.Drawing.Layers;

/// <summary>
/// Represents a layer managed by the <see cref="Canvas"/>
/// </summary>
[Serializable]
public class CanvasLayer
{
  /// <summary>
  /// Backing fields for properties
  /// </summary>
  protected BackingFields _BackingFields = new BackingFields();

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
  /// A <see cref="Rectangle"/> that represents the area of this <see cref="CanvasLayer"/>
  /// </summary>
  public Rectangle Rect => new Rectangle(0, 0, Width, Height);

  /// <summary>
  /// Name of Layer
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Override to return the <see cref="Name"/>
  /// </summary>
  /// <returns><see cref="Name"/></returns>
  public override string ToString() => Name;

  /// <summary>
  /// Draws the layer background and all non-selected objects if the layer is visible.
  /// <para>
  /// <paramref name="graphics"/> is the drawing surface.
  /// <paramref name="snapInterval"/> is the grid snap interval for object alignment.
  /// </para>
  /// </summary>
  public virtual void Draw(Graphics graphics, int snapInterval)
  {
    if (IsVisible)
    {
      // Draw background
      graphics.FillRectangle(new SolidBrush(BackgroundColor), Rect);
    }
  }
}
