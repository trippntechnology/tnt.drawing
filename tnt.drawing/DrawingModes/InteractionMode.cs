using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Base class for drawing modes that interact with existing objects on the canvas.
/// Provides common functionality for object selection, tracking, and feedback.
/// </summary>
/// <remarks>
/// Constructor for ObjectInteractionMode
/// </remarks>
/// <param name="canvas">The canvas to interact with</param>
/// <param name="layer">The canvas layer containing objects</param>
public abstract class InteractionMode(Canvas canvas, CanvasLayer layer) : DrawingMode(canvas, layer)
{
  /// <summary>
  /// The object currently under the mouse cursor
  /// </summary>
  protected CanvasObject? _objectUnderMouse = null;

  /// <summary>
  /// The last known mouse position
  /// </summary>
  protected Point _lastMouseLocation = Point.Empty;

  /// <summary>
  /// Sets the Canvas cursor based on the object under mouse
  /// </summary>
  protected virtual void UpdateFeedback(Point location, Keys modifierKeys)
  {
    var feedback = _objectUnderMouse?.GetFeedback(location, modifierKeys) ?? Feedback.Default;
    Canvas.OnFeedbackChanged(feedback);
  }

  /// <summary>
  /// Sets the Canvas cursor associated with the object under mouse
  /// </summary>
  public override void OnKeyDown(KeyEventArgs e)
  {
    UpdateFeedback(_lastMouseLocation, e.Modifiers);
    base.OnKeyDown(e);
  }

  /// <summary>
  /// Sets the Canvas cursor associated with the object under mouse
  /// </summary>
  public override void OnKeyUp(KeyEventArgs e)
  {
    UpdateFeedback(_lastMouseLocation, e.Modifiers);
    base.OnKeyUp(e);
  }

  /// <summary>
  /// Updates the previous mouse location and object under mouse
  /// </summary>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    // Update the saved mouse location
    var location = Canvas.SnapToInterval && modifierKeys.DoesNotContain(Keys.Control) ? e.Location.Snap(Canvas.SnapInterval) : e.Location;
    _lastMouseLocation = location;

    if (!IsMouseDown)
    {
      // Update object under mouse when not dragging
      _objectUnderMouse = FindObjectAt(Layer.CanvasObjects, e.Location, modifierKeys);
      UpdateFeedback(e.Location, modifierKeys);
    }

    base.OnMouseMove(e, modifierKeys);
  }

  /// <summary>
  /// Resets the <see cref="DrawingMode"/> and <see cref="DrawingMode.Layer"/>
  /// </summary>
  public override void Reset()
  {
    Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
    base.Reset();
  }

  /// <summary>
  /// Finds the first selected object under the mouse, or the topmost object in the given list at the mouse location.
  /// </summary>
  protected virtual CanvasObject? FindObjectAt(List<CanvasObject> objs, Point mouseLocation, Keys modifierKeys)
  {
    var selectedObjects = objs.FindAll(o => o.IsSelected);
    return selectedObjects.FirstOrDefault(o => o.MouseOver(mouseLocation, modifierKeys).HitObject != null) ??
      objs.LastOrDefault(o => o.MouseOver(mouseLocation, modifierKeys).HitObject != null);
  }
}