using System;
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
/// Provides a drawing mode for selecting and moving objects on the canvas.
/// </summary>
public class SelectMode(ObjectLayer layer) : DrawingMode(layer)
{
  private bool _allowMove = false;
  private readonly List<CanvasObject> _activeObjects = new();
  private Point _lastMouseLocation = Point.Empty;
  private CanvasObject? _objectUnderMouse = null;

  // Selection box state
  private bool _isSelecting = false;
  private Point _selectionAnchor = Point.Empty;
  private Point _selectionCurrent = Point.Empty;

  /// <summary>
  /// Draws the selected objects and selection box if active.
  /// </summary>
  public override void OnDraw(Graphics graphics, Canvas canvas)
  {
    var selectedObjects = Layer.CanvasObjects.FindAll(o => o.IsSelected);
    selectedObjects.ForEach(o => o.Draw(graphics));
    // Draw selection rectangle if active
    if (_isSelecting)
    {
      var rect = GetSelectionRectangle();
      using var pen = new Pen(Color.Blue) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
      graphics.DrawRectangle(pen, rect);
    }
    base.OnDraw(graphics, canvas);
  }

  /// <summary>
  /// Handles key down events in selection mode, updating feedback for the object under the mouse and passing the event to the base drawing mode.
  /// <para>
  /// This method is called with the cursor position in grid coordinates and any active modifier keys. If an object is under the mouse, its feedback is retrieved and sent to the canvas for UI updates (cursor, hint).
  /// </para>
  /// <param name="cursorPosition">The cursor position in grid (world) coordinates.</param>
  /// <param name="modifiers">The active modifier keys (e.g., Control, Shift).</param>
  /// <param name="canvas">The target <see cref="Canvas"/> instance.</param>
  /// </summary>
  public override void OnKeyDown(Point cursorPosition, Keys modifiers, Canvas canvas)
  {
    base.OnKeyDown(cursorPosition, modifiers, canvas);
    _objectUnderMouse?.GetFeedback(cursorPosition, modifiers)?.Also(f => canvas.OnFeedbackChanged(f));
  }

  /// <summary>
  /// Handles key up events in selection mode, updating feedback for the object under the mouse and passing the event to the base drawing mode.
  /// <para>
  /// This method is called with the cursor position in grid coordinates and any active modifier keys. If an object is under the mouse, its feedback is retrieved and sent to the canvas for UI updates (cursor, hint).
  /// </para>
  /// <param name="cursorPosition">The cursor position in grid (world) coordinates.</param>
  /// <param name="modifiers">The active modifier keys (e.g., Control, Shift).</param>
  /// <param name="canvas">The target <see cref="Canvas"/> instance.</param>
  /// </summary>
  public override void OnKeyUp(Point cursorPosition, Keys modifiers, Canvas canvas)
  {
    _objectUnderMouse?.GetFeedback(cursorPosition, modifiers)?.Also(f => canvas.OnFeedbackChanged(f));
    base.OnKeyUp(cursorPosition, modifiers, canvas);
  }

  /// <summary>
  /// Handles mouse down event for selection.
  /// </summary>
  public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys, Canvas canvas)
  {
    base.OnMouseDown(e, modifierKeys, canvas);

    if (_objectUnderMouse == null)
    {
      // Start selection box
      _isSelecting = true;
      _selectionAnchor = e.Location;
      _selectionCurrent = e.Location;
      if ((modifierKeys & Keys.Control) != Keys.Control) Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
      _activeObjects.Clear();
    }
    else
    {
      _isSelecting = false;
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

    Layer.CanvasObjects.FindAll(o => o.IsSelected)?.Cast<object>().ToList()?.Also(o => canvas.OnObjectsSelected(o));
    canvas.Invalidate();
    UpdateFeedback(e.Location, modifierKeys, canvas);
  }

  /// <summary>
  /// Handles mouse movement during selection mode. Moves selected objects if allowed, otherwise updates feedback and object under mouse.
  /// </summary>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys, Canvas canvas)
  {
    var location = canvas.SnapToInterval == modifierKeys.DoesNotContain(Keys.Control) ? e.Location.Snap(canvas.SnapInterval) : e.Location;

    if (_isSelecting && _isMouseDown)
    {
      _selectionCurrent = location;
      canvas.Invalidate();
    }
    else if (_isMouseDown && _allowMove)
    {
      var dx = location.X - _lastMouseLocation.X;
      var dy = location.Y - _lastMouseLocation.Y;
      _activeObjects.ForEach(o => o.Move(new MoveInfo(location, dx, dy), modifierKeys));
      canvas.Invalidate();
    }
    else
    {
      _objectUnderMouse = GetObjectUnderMouse(e.Location, modifierKeys);
      UpdateFeedback(e.Location, modifierKeys, canvas);
    }
    _lastMouseLocation = location;
    base.OnMouseMove(e, modifierKeys, canvas);
  }

  /// <summary>
  /// Handles mouse up events, ending any ongoing selection or move operations.
  /// </summary>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys, Canvas canvas)
  {
    if (_isSelecting)
    {
      _isSelecting = false;
      var rect = GetSelectionRectangle();

      using (var region = new Region(rect))
      {
        Layer.CanvasObjects.ForEach(o => o.IsSelected = (modifierKeys == Keys.Control && o.IsSelected) || o.IntersectsWith(region));
      }

      canvas.OnObjectsSelected(Layer.CanvasObjects.Where(o => o.IsSelected).Cast<object>().ToList());
      canvas.Invalidate();
    }
    base.OnMouseUp(e, modifierKeys, canvas);
  }

  /// <summary>
  /// Clears the current selection and resets the active objects in selection mode.
  /// </summary>
  public override void Reset(Canvas canvas)
  {
    _activeObjects.Clear();
    Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
    _isSelecting = false;
    base.Reset(canvas);
  }

  /// <summary>
  /// Updates feedback cursor and tooltip based on what's under the mouse.
  /// </summary>
  protected void UpdateFeedback(Point location, Keys modifierKeys, Canvas canvas)
  {
    var feedback = _objectUnderMouse?.GetFeedback(location, modifierKeys) ?? Feedback.Default;
    canvas.OnFeedbackChanged(feedback);
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

  /// <summary>
  /// Returns the selection rectangle in normalized coordinates.
  /// </summary>
  private Rectangle GetSelectionRectangle()
  {
    int x1 = Math.Min(_selectionAnchor.X, _selectionCurrent.X);
    int y1 = Math.Min(_selectionAnchor.Y, _selectionCurrent.Y);
    int x2 = Math.Max(_selectionAnchor.X, _selectionCurrent.X);
    int y2 = Math.Max(_selectionAnchor.Y, _selectionCurrent.Y);
    return new Rectangle(x1, y1, x2 - x1, y2 - y1);
  }
}
