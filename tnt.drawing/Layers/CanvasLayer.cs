using Newtonsoft.Json;
using System.Drawing;
using TNT.Reactive;

namespace TNT.Drawing.Layers;

/// <summary>
/// Represents a layer managed by the <see cref="Canvas"/>
/// </summary>
public class CanvasLayer
{
  /// <summary>
  /// Backing fields for properties
  /// </summary>
  protected BackingFields _BackingFields = new BackingFields();
  private Rectangle _backgroundRect;

  /// <summary>
  /// Indicates whether a property managed by <see cref="_BackingFields"/> has changed and the layer needs to be repainted.
  /// - Initialized to <c>true</c> so the layer is drawn on first render.
  /// - Set to <c>true</c> whenever a property managed by <see cref="_BackingFields"/> changes (e.g., color, dimensions, visibility).
  /// - Used in <see cref="Draw"/> to determine if the layer bitmap should be regenerated.
  /// - Reset to <c>false</c> after redrawing, so the layer is only regenerated when necessary.
  /// </summary>
  protected bool _backingFieldChanged = true;

  /// <summary>
  /// Indicates whether the <see cref="CanvasLayer"/> is visisble
  /// </summary>
  [JsonIgnore]
  public bool IsVisible { get; set; } = true;

  /// <summary>
  /// Background color 
  /// </summary>
  public Color BackgroundColor { get => _BackingFields.Get<Color>(Color.Transparent); set => _BackingFields.Set(value); }

  /// <summary>
  /// The width of the <see cref="CanvasLayer"/>
  /// </summary>
  [JsonIgnore]
  public int Width { get => _BackingFields.Get(1024); set => _BackingFields.Set(value); }

  /// <summary>
  /// The height of the <see cref="CanvasLayer"/>
  /// </summary>
  [JsonIgnore]
  public int Height { get => _BackingFields.Get(768); set => _BackingFields.Set(value); }

  /// <summary>
  /// Name of Layer
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Override to return the <see cref="Name"/>
  /// </summary>
  /// <returns><see cref="Name"/></returns>
  public override string ToString() => Name;

  // Constructors
  /// <summary>
  /// Initializes a new instance of <see cref="GridLayer"/>.
  /// Subscribes to property change notifications to trigger grid redraws when properties change.
  /// </summary>
  public CanvasLayer()
  {
    _BackingFields.OnFieldChanged += (field, value) => { _backingFieldChanged = true; };
  }

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
      if (_backingFieldChanged)
      {
        _backgroundRect = new Rectangle(0, 0, Width, Height);
      }
      // Draw background
      graphics.FillRectangle(new SolidBrush(BackgroundColor), _backgroundRect);
    }
  }
}
