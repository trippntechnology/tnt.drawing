using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;
using TNT.Drawing.Model;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Rotation <see cref="DrawingMode"/> for rotating objects on the canvas.
/// </summary>
public class RotationMode(Canvas canvas, CanvasLayer layer) : DrawingMode(canvas, layer)
{
  private CanvasObject? objectUnderMouse = null;

  /// <summary>
  /// Checks if the mouse is over any object in the given list. If so, persists that object in <see cref="objectUnderMouse"/>.
  /// </summary>
  /// <param name="objs">List of <see cref="CanvasObject"/> to check</param>
  /// <param name="mouseLocation">Current mouse location</param>
  /// <param name="modifierKeys">Current keyboard modifier keys</param>
  /// <returns>The object under the mouse, or null if none</returns>
  protected CanvasObject? FindObjectAt(List<CanvasObject> objs, Point mouseLocation, Keys modifierKeys)
  {
    var found = objs.LastOrDefault(o => o.MouseOver(mouseLocation, modifierKeys).HitObject != null);
    objectUnderMouse = found;
    return found;
  }

  /// <summary>
  /// Handles mouse movement: updates the object under the mouse and provides feedback.
  /// </summary>
  /// <param name="e">Mouse event arguments</param>
  /// <param name="modifierKeys">Current keyboard modifier keys</param>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    var location = Canvas.SnapToInterval == modifierKeys.DoesNotContain(Keys.Control) ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    // Update the object under the mouse
    objectUnderMouse = Layer.CanvasObjects.LastOrDefault(o => o.MouseOver(location, modifierKeys).HitObject != null);

    // Provide feedback: set cursor to hand if over an object, else default
    var feedback = objectUnderMouse?.GetFeedback(location, modifierKeys) ?? Feedback.Default;
    Canvas.OnFeedbackChanged(feedback);

    base.OnMouseMove(e, modifierKeys);
  }

  /// <summary>
  /// Handles mouse down event: selects the object under the mouse if present.
  /// </summary>
  /// <param name="e">Mouse event arguments</param>
  /// <param name="modifierKeys">Current keyboard modifier keys</param>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
  {
    Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
    objectUnderMouse?.Also(o => o.IsSelected = true);

    if (objectUnderMouse != null)
    {
      Canvas.OnObjectsSelected(new List<object> { objectUnderMouse });
    }

    Canvas.Invalidate();
    base.OnMouseUp(e, modifierKeys);
  }

  public override void OnDraw(Graphics graphics)
  {
    base.OnDraw(graphics);
    if (objectUnderMouse?.IsSelected == true)
    {
      // Draw the selected object
      objectUnderMouse.Draw(graphics);

      // Calculate the center of mass (centroid) for the object
      // Assumes CanvasObject exposes a method or property for centroid; otherwise, fallback to bounding box center
      Point? center = objectUnderMouse.GetCentroid();

      if (center == null) return;

      // Fallback: use bounding rectangle center
      //var bounds = objectUnderMouse.GetBounds();
      //center = new PointF(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height / 2f);

      // Draw a filled circle at the center
      const float radius = 6f;
      using (var brush = new SolidBrush(Color.Red))
      {
        graphics.FillEllipse(brush, center.Value.X - radius, center.Value.Y - radius, radius * 2, radius * 2);
      }
    }
  }
}
