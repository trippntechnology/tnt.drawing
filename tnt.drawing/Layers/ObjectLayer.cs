using System.Collections.Generic;
using System.Drawing;
using TNT.Drawing.Objects;

namespace TNT.Drawing.Layers;

/// <summary>
/// Represents a layer that manages and renders a collection of <see cref="CanvasObject"/> instances.
/// Supports selection, alignment, and z-order manipulation of objects.
/// </summary>
public class ObjectLayer : CanvasLayer
{
  // Properties
  /// <summary>
  /// The collection of <see cref="CanvasObject"/> instances managed by this layer.
  /// </summary>
  public List<CanvasObject> CanvasObjects { get; set; } = new List<CanvasObject>();

  // Methods

  /// <summary>
  /// Returns all selected <see cref="CanvasObject"/> instances in this layer.
  /// </summary>
  /// <returns>A list of selected <see cref="CanvasObject"/> objects.</returns>
  public virtual List<CanvasObject> GetSelected() => CanvasObjects.FindAll(o => o.IsSelected);

  /// <summary>
  /// Draws all non-selected <see cref="CanvasObject"/> instances in this layer if the layer is visible.
  /// Selected objects are typically drawn separately for highlighting.
  /// </summary>
  /// <param name="graphics">The graphics context to draw on.</param>
  /// <param name="snapInterval">The grid snap interval for object alignment.</param>
  public override void Draw(Graphics graphics, int snapInterval)
  {
    base.Draw(graphics, snapInterval);

    if (IsVisible)
    {
      // Draw objects
      CanvasObjects?.ForEach(o => { if (!o.IsSelected) o.Draw(graphics); });
    }
  }

  /// <summary>
  /// Aligns all selected <see cref="CanvasObject"/> instances to the specified grid snap interval.
  /// </summary>
  /// <param name="snapInterval">The grid snap interval to align objects to.</param>
  public virtual void AlignToSnapInterval(int snapInterval)
  {
    GetSelected().ForEach(o => o.Align(snapInterval));
  }

  /// <summary>
  /// Moves all selected <see cref="CanvasObject"/> instances to the front of the z-order in this layer.
  /// </summary>
  public virtual void BringToFront()
  {
    GetSelected().ForEach(obj =>
    {
      CanvasObjects.Remove(obj);
      CanvasObjects.Add(obj);
    });
  }
}
