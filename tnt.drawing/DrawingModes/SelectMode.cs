using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Select <see cref="DrawingMode"/> for selecting and moving objects on the canvas.
/// </summary>
/// <remarks>
/// Constructor for SelectMode
/// </remarks>
/// <param name="canvas">The canvas to interact with</param>
/// <param name="layer">The canvas layer containing objects</param>
public class SelectMode(Canvas canvas, CanvasLayer layer) : DrawingMode(canvas, layer)
{
  private List<CanvasObject> _activeObjects = new List<CanvasObject>();
  private bool _allowMove = false;
  private CanvasObject? _innerHitObject = null;
  private CanvasObject? _hitObject = null;
  private CanvasObject? _objectUnderMouse = null;
  private Point _lastMouseLocation = Point.Empty;

  /// <summary>
  /// Handles mouse down event for selection
  /// </summary>
  public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
  {
    // Call base.OnMouseDown to ensure proper mouse down state tracking
    base.OnMouseDown(e, modifierKeys);

    if (_objectUnderMouse == null)
    {
      Layer.CanvasObjects.ForEach(o => o.IsSelected = o == _objectUnderMouse);
    }
    else
    {
      var (_, innerHitObject, allowMove) = _objectUnderMouse.OnMouseDown(e.Location, modifierKeys);
      _allowMove = allowMove;

      _activeObjects.Clear();

      if (innerHitObject != null)
      {
        Layer.CanvasObjects.ForEach(o => o.IsSelected = o == _objectUnderMouse);
        _activeObjects.Add(innerHitObject);
      }
      else
      {
        if (modifierKeys == Keys.Control)
        {
          _objectUnderMouse.IsSelected = !_objectUnderMouse.IsSelected;
          _allowMove = false;
        }
        else if (!_objectUnderMouse.IsSelected)
        {
          Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
          _objectUnderMouse.IsSelected = true;
        }

        _activeObjects.AddRange(Layer.CanvasObjects.FindAll(o => o.IsSelected));
      }
    }

    Layer.CanvasObjects.FindAll(o => o.IsSelected)?.Cast<object>().ToList()?.Also(o => Canvas.OnObjectsSelected(o));

    Canvas.Invalidate();

    // Update the feedback
    UpdateFeedback(e.Location, modifierKeys);
  }

  /// <summary>
  /// Handles mouse movement during selection mode. When the mouse is down and movement is allowed,
  /// it moves selected objects by the distance the mouse has moved. Otherwise, updates feedback and object under mouse.
  /// </summary>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    var location = Canvas.SnapToInterval == modifierKeys.DoesNotContain(Keys.Control) ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    if (IsMouseDown && _allowMove)
    {
      var dx = location.X - _lastMouseLocation.X;
      var dy = location.Y - _lastMouseLocation.Y;

      _activeObjects.ForEach(o => o.MoveBy(dx, dy, modifierKeys));

      Canvas.Invalidate();
    }
    else
    {
      // Update object under mouse when not dragging
      _objectUnderMouse = GetObjectUnderMouse(e.Location, modifierKeys);
      //TNTLogger.Info($"Object under mouse: {_objectUnderMouse?.ToString() ?? "None"}");
      UpdateFeedback(e.Location, modifierKeys);
    }

    // Update the saved mouse location
    _lastMouseLocation = location;

    base.OnMouseMove(e, modifierKeys);
  }

  /// <summary>
  /// Finds the topmost <see cref="CanvasObject"/> at the specified location, prioritizing selected objects.
  /// Returns the first selected object found that is under the mouse, or if none, the first unselected object found.
  /// </summary>
  /// <param name="location">The location to test for object presence.</param>
  /// <param name="modifierKeys">The current keyboard modifier keys.</param>
  /// <returns>The <see cref="CanvasObject"/> under the specified location, or null if none found.</returns>
  private CanvasObject? GetObjectUnderMouse(Point location, Keys modifierKeys)
  {
    var canvasObjects = Layer.CanvasObjects;
    var selectedObjects = canvasObjects.FindAll(o => o.IsSelected);

    return selectedObjects.LastOrDefault(o => o.IsMouseOver(location, modifierKeys)) ?? canvasObjects.LastOrDefault(o => o.IsMouseOver(location, modifierKeys));
  }

  /// <summary>
  /// Handles mouse up events, ending any ongoing operations
  /// </summary>
  /// <param name="e">Contains mouse button information</param>
  /// <param name="modifierKeys">Current keyboard modifier keys being pressed</param>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
  {
    Canvas.Invalidate();
    base.OnMouseUp(e, modifierKeys);
  }

  /// <summary>
  /// Draws the selected objects
  /// </summary>
  public override void OnDraw(Graphics graphics)
  {
    // Draw all selected objects
    var selectedObjects = Layer.CanvasObjects.FindAll(o => o.IsSelected);
    selectedObjects.ForEach(o => o.Draw(graphics));
    base.OnDraw(graphics);
  }

  /// <summary>
  /// Updates feedback cursor and tooltip based on what's under the mouse
  /// </summary>
  protected void UpdateFeedback(Point location, Keys modifierKeys)
  {
    // Default behavior for other areas
    var feedback = _objectUnderMouse?.GetFeedback(location, modifierKeys) ?? Feedback.Default;
    Canvas.OnFeedbackChanged(feedback);
  }
}
