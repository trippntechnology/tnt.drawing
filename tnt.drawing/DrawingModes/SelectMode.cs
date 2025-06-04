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
/// Select <see cref="DrawingMode"/>
/// </summary>
public class SelectMode(Canvas canvas, CanvasLayer layer) : DrawingMode(canvas, layer)
{
  private bool allowMove = false;
  private Point previousMouseLocation = Point.Empty;
  private List<CanvasObject> selectedObjects = new List<CanvasObject>();
  private CanvasObject? objectUnderMouse = null;

  /// <summary>
  /// Resets the <see cref="DrawingMode"/> and <see cref="DrawingMode.Layer"/>
  /// </summary>
  public override void Reset()
  {
    Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
    base.Reset();
  }

  /// <summary>
  /// TODO
  /// </summary>
  public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
  {
    var (hitObject, childObject, allowMove) = objectUnderMouse?.OnMouseDown(e.Location, modifierKeys) ?? MouseDownResponse.Default;

    this.allowMove = allowMove;

    if (childObject == null)
    {
      if (hitObject != null && objectUnderMouse != hitObject) selectedObjects.Clear();

      if (objectUnderMouse == null)
      {
        selectedObjects.Clear();
      }
      else if (modifierKeys == Keys.Control && selectedObjects.Contains(objectUnderMouse))
      {
        selectedObjects.Remove(objectUnderMouse);
      }
      else if (modifierKeys == Keys.Control && !selectedObjects.Contains(objectUnderMouse))
      {
        selectedObjects.Add(objectUnderMouse);
      }
      else if (!selectedObjects.Contains(objectUnderMouse))
      {
        selectedObjects.Clear();
        selectedObjects.Add(objectUnderMouse);
      }
    }
    else if (hitObject != null)
    {
      // Only hitObject should be selected at this point
      selectedObjects.Clear();
      selectedObjects.Add(hitObject);
    }

    Canvas.OnObjectsSelected(selectedObjects.Select(o => o as object).ToList());

    // Select/unselect objects
    Layer.CanvasObjects.ForEach(o => o.IsSelected = selectedObjects.Contains(o));

    Canvas.Refresh();

    // Set the feedback (cursor) for the object under mouse
    var feedback = objectUnderMouse?.GetFeedback(e.Location, modifierKeys) ?? Feedback.Default;
    Canvas.OnFeedbackChanged(feedback);


    base.OnMouseDown(e, modifierKeys);
  }

  /// <summary>
  /// Sets the <see cref="Canvas"/> cursor associated with the object under mouse
  /// </summary>
  public override void OnKeyDown(KeyEventArgs e)
  {
    var feedback = objectUnderMouse?.GetFeedback(previousMouseLocation, e.Modifiers) ?? Feedback.Default;
    Canvas.OnFeedbackChanged(feedback);
    base.OnKeyDown(e);
  }

  /// <summary>
  /// Sets the <see cref="Canvas"/> cursor associated with the object under mouse
  /// </summary>
  /// <param name="e"></param>
  public override void OnKeyUp(KeyEventArgs e)
  {
    var feedback = objectUnderMouse?.GetFeedback(previousMouseLocation, e.Modifiers) ?? Feedback.Default;
    Canvas.OnFeedbackChanged(feedback);
    base.OnKeyUp(e);
  }

  /// <summary>
  /// Handles mouse movement during selection mode. When the mouse is down and movement is allowed,
  /// it moves selected objects by the distance the mouse has moved. When the mouse is up,
  /// it identifies objects under the cursor and updates the feedback (cursor and hint).
  /// </summary>
  /// <param name="e">Contains mouse position and button information</param>
  /// <param name="modifierKeys">Current keyboard modifier keys being pressed</param>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    var location = Canvas.SnapToInterval == modifierKeys.DoesNotContain(Keys.Control) ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    var dx = location.X - previousMouseLocation.X;
    var dy = location.Y - previousMouseLocation.Y;
    previousMouseLocation = location;

    if (IsMouseDown)
    {
      if (allowMove)
      {
        selectedObjects.ForEach(o => o.MoveBy(dx, dy, modifierKeys));
        Canvas.Invalidate();
      }
    }
    else
    {
      objectUnderMouse = FindObjectAt(Layer.CanvasObjects, e.Location, modifierKeys);
      var feedback = objectUnderMouse?.GetFeedback(e.Location, modifierKeys) ?? Feedback.Default;
      Canvas.OnFeedbackChanged(feedback);
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

  /// <summary>
  /// Finds a <see cref="CanvasObject"/> at <paramref name="mouseLocation"/>
  /// </summary>
  /// <param name="objs"></param>
  /// <param name="mouseLocation"></param>
  /// <param name="modifierKeys"></param>
  /// <returns><see cref="CanvasObject"/> if found, otherwise false</returns>
  protected CanvasObject? FindObjectAt(List<CanvasObject> objs, Point mouseLocation, Keys modifierKeys)
  {
    // See if where over a selected object first just in case the selected object is in the list
    // after another object in the same place
    return selectedObjects.FirstOrDefault(o => o.MouseOver(mouseLocation, modifierKeys).HitObject != null) ??
      objs.FirstOrDefault(o => o.MouseOver(mouseLocation, modifierKeys).HitObject != null);
  }
}
