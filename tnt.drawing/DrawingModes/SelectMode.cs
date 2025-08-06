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
public class SelectMode : DrawingMode
{
  // Fields
  private readonly List<CanvasObject> _activeObjects = new();
  private CanvasObject? _objectUnderMouse = null;
  private bool _allowMove = false;
  private Point _lastMouseLocation = Point.Empty;

  /// <summary>
  /// Initializes a new instance of the <see cref="SelectMode"/> class.
  /// </summary>
  /// <param name="canvas">The canvas to interact with.</param>
  /// <param name="layer">The canvas layer containing objects.</param>
  public SelectMode(Canvas canvas, CanvasLayer layer) : base(canvas, layer) { }

  /// <summary>
  /// Handles mouse down event for selection.
  /// </summary>
  public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
  {
    base.OnMouseDown(e, modifierKeys);

    if (_objectUnderMouse == null)
    {
      Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
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
    UpdateFeedback(e.Location, modifierKeys);
  }

  /// <summary>
  /// Handles mouse movement during selection mode. Moves selected objects if allowed, otherwise updates feedback and object under mouse.
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
      _objectUnderMouse = GetObjectUnderMouse(e.Location, modifierKeys);
      UpdateFeedback(e.Location, modifierKeys);
    }
    _lastMouseLocation = location;
    base.OnMouseMove(e, modifierKeys);
  }

  /// <summary>
  /// Handles mouse up events, ending any ongoing operations.
  /// </summary>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
  {
    Canvas.Invalidate();
    base.OnMouseUp(e, modifierKeys);
  }

  /// <summary>
  /// Draws the selected objects.
  /// </summary>
  public override void OnDraw(Graphics graphics)
  {
    var selectedObjects = Layer.CanvasObjects.FindAll(o => o.IsSelected);
    selectedObjects.ForEach(o => o.Draw(graphics));
    base.OnDraw(graphics);
  }

  /// <summary>
  /// Updates feedback cursor and tooltip based on what's under the mouse.
  /// </summary>
  protected void UpdateFeedback(Point location, Keys modifierKeys)
  {
    var feedback = _objectUnderMouse?.GetFeedback(location, modifierKeys) ?? Feedback.Default;
    Canvas.OnFeedbackChanged(feedback);
  }

  /// <summary>
  /// Finds the topmost <see cref="CanvasObject"/> at the specified location, prioritizing selected objects.
  /// </summary>
  private CanvasObject? GetObjectUnderMouse(Point location, Keys modifierKeys)
  {
    var canvasObjects = Layer.CanvasObjects;
    var selectedObjects = canvasObjects.FindAll(o => o.IsSelected);
    return selectedObjects.LastOrDefault(o => o.IsMouseOver(location, modifierKeys)) ?? canvasObjects.LastOrDefault(o => o.IsMouseOver(location, modifierKeys));
  }
}
