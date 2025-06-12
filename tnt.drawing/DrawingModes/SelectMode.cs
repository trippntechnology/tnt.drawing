using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Select <see cref="DrawingMode"/> for selecting and moving objects on the canvas.
/// </summary>
/// <remarks>
/// Constructor for SelectMode
/// </remarks>
/// <param name="canvas">The canvas to interact with</param>
/// <param name="layer">The canvas layer containing objects</param>
public class SelectMode(Canvas canvas, CanvasLayer layer) : InteractionMode(canvas, layer)
{
  private bool _allowMove = false;

  /// <summary>
  /// Handles mouse down event for selection
  /// </summary>
  public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
  {
    var (hitObject, childObject, allowMove) = _objectUnderMouse?.OnMouseDown(e.Location, modifierKeys) ?? MouseDownResponse.Default;

    _allowMove = allowMove;

    if (childObject == null)
    {
      if (_objectUnderMouse == null)
      {
        Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
      }
      else if (modifierKeys == Keys.Control)
      {
        _objectUnderMouse.IsSelected = !_objectUnderMouse.IsSelected;
      }
      else if (!_objectUnderMouse.IsSelected)
      {
        Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
        _objectUnderMouse.IsSelected = true;
      }
    }

    Layer.CanvasObjects.FindAll(o => o.IsSelected)?.Cast<object>().ToList()?.Also(o => Canvas.OnObjectsSelected(o));

    Canvas.Invalidate();

    // Update the feedback
    UpdateFeedback(e.Location, modifierKeys);

    base.OnMouseDown(e, modifierKeys);
  }

  /// <summary>
  /// Handles mouse movement during selection mode. When the mouse is down and movement is allowed,
  /// it moves selected objects by the distance the mouse has moved.
  /// </summary>
  /// <param name="e">Contains mouse position and button information</param>
  /// <param name="modifierKeys">Current keyboard modifier keys being pressed</param>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    var location = Canvas.SnapToInterval == modifierKeys.DoesNotContain(Keys.Control) ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    var dx = location.X - _lastMouseLocation.X;
    var dy = location.Y - _lastMouseLocation.Y;

    if (IsMouseDown)
    {
      if (_allowMove)
      {
        Layer.CanvasObjects.FindAll(o => o.IsSelected).ForEach(o => o.MoveBy(dx, dy, modifierKeys));
        Canvas.Invalidate();
      }
    }

    base.OnMouseMove(e, modifierKeys);
  }

  /// <summary>
  /// Draws the selected objects
  /// </summary>
  public override void OnDraw(Graphics graphics)
  {
    // Draw selected objects
    Layer.CanvasObjects.FindAll(o => o.IsSelected).ForEach(o => o.Draw(graphics));
    base.OnDraw(graphics);
  }
}
