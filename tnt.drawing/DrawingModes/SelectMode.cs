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
public class SelectMode(Canvas canvas, CanvasLayer layer) : DrawingMode(canvas, layer)
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
  public override void OnDraw(Graphics graphics)
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
    base.OnDraw(graphics);
  }

  /// <summary>
  /// Handles mouse down event for selection.
  /// </summary>
  public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
  {
    base.OnMouseDown(e, modifierKeys);

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

    if (_isSelecting && IsMouseDown)
    {
      _selectionCurrent = location;
      Canvas.Invalidate();
    }
    else if (IsMouseDown && _allowMove)
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
    if (_isSelecting)
    {
      _isSelecting = false;
      var rect = GetSelectionRectangle();
      // Select all objects whose bounding box intersects the selection rectangle
      var selected = Layer.CanvasObjects.Where(o => rect.IntersectsWith(GetObjectBounds(o))).ToList();
      Layer.CanvasObjects.ForEach(o => o.IsSelected = (modifierKeys == Keys.Control && o.IsSelected) || selected.Contains(o));
      Canvas.OnObjectsSelected(selected.Cast<object>().ToList());
      Canvas.Invalidate();
    }
    base.OnMouseUp(e, modifierKeys);
  }

  /// <summary>
  /// Clears the current selection and resets the active objects in selection mode.
  /// </summary>
  public override void Reset()
  {
    _activeObjects.Clear();
    Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
    _isSelecting = false;
    base.Reset();
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

  /// <summary>
  /// Gets the bounding rectangle of a CanvasObject (override for more precise hit if needed).
  /// </summary>
  private Rectangle GetObjectBounds(CanvasObject obj)
  {
    // If CanvasObject exposes a bounding box, use it. Otherwise, fallback to a default.
    // Here, we assume Draw uses the object's bounding box.
    // If CanvasObject has a GetBounds() method, use that instead.
    var centroid = obj.GetCentroidPosition();
    if (centroid.HasValue)
      return new Rectangle(centroid.Value.X - 10, centroid.Value.Y - 10, 20, 20); // fallback size
    return new Rectangle(0, 0, 0, 0); // fallback
  }
}
