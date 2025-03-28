using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Drawing.Extentions;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;
using TNT.Drawing.Resource;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Select <see cref="DrawingMode"/>
/// </summary>
public class SelectMode : DrawingMode
{
  private bool allowMove = false;
  private Point previousMouseLocation = Point.Empty;
  private List<CanvasObject> selectedObjects = new List<CanvasObject>();
  private CanvasObject? objectUnderMouse = null;

  /// <summary>
  /// Initializes with a <paramref name="layer"/>
  /// </summary>
  public SelectMode(CanvasLayer layer) : base(layer) { }

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
    var activeObject = objectUnderMouse?.OnMouseDown(e.Location, modifierKeys, out allowMove);

    if (activeObject != null && objectUnderMouse != activeObject) selectedObjects.Clear();

    if (objectUnderMouse == null)
    {
      selectedObjects.Clear();
    }
    else if (modifierKeys == Keys.Shift && selectedObjects.Contains(objectUnderMouse))
    {
      selectedObjects.Remove(objectUnderMouse);
    }
    else if (modifierKeys == Keys.Shift && !selectedObjects.Contains(objectUnderMouse))
    {
      selectedObjects.Add(objectUnderMouse);
    }
    else if (!selectedObjects.Contains(objectUnderMouse))
    {
      selectedObjects.Clear();
      selectedObjects.Add(objectUnderMouse);
    }

    Canvas?.OnObjectsSelected(selectedObjects.Select(o => o as object).ToList());

    // Select/unselect objects
    Layer.CanvasObjects.ForEach(o => o.IsSelected = selectedObjects.Contains(o));

    Refresh(Layer);

    base.OnMouseDown(e, modifierKeys);
  }

  /// <summary>
  /// Sets the <see cref="Canvas"/> cursor associated with the object under mouse
  /// </summary>
  public override void OnKeyDown(KeyEventArgs e)
  {
    var feedback = objectUnderMouse?.GetFeedback(previousMouseLocation, e.Modifiers) ?? new Feedback(Cursors.Default, string.Empty);
    Canvas?.OnFeedbackChanged(feedback.Cursor, feedback.Hint);
    base.OnKeyDown(e);
  }

  /// <summary>
  /// Sets the <see cref="Canvas"/> cursor associated with the object under mouse
  /// </summary>
  /// <param name="e"></param>
  public override void OnKeyUp(KeyEventArgs e)
  {
    var feedback = objectUnderMouse?.GetFeedback(previousMouseLocation, e.Modifiers) ?? new Feedback(Cursors.Default, string.Empty);
    Canvas?.OnFeedbackChanged(feedback.Cursor, feedback.Hint);
    base.OnKeyUp(e);
  }

  /// <summary>
  /// TODO
  /// </summary>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    var location = Canvas?.SnapToInterval == true && (modifierKeys & Keys.Control) != Keys.Control ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    var dx = location.X - previousMouseLocation.X;
    var dy = location.Y - previousMouseLocation.Y;
    previousMouseLocation = location;

    objectUnderMouse = FindObjectAt(Layer.CanvasObjects, e.Location, modifierKeys);

    if (IsMouseDown && allowMove) selectedObjects.ForEach(o => o.MoveBy(dx, dy, modifierKeys));

    var feedback = objectUnderMouse?.GetFeedback(e.Location, modifierKeys) ?? new Feedback(Cursors.Default, string.Empty);
    Canvas?.OnFeedbackChanged(feedback.Cursor, feedback.Hint);

    Refresh(this.Layer);
    base.OnMouseMove(e, modifierKeys);
  }

  /// <summary>
  /// Draws the selected objects
  /// </summary>
  public override void OnPaint(PaintEventArgs e)
  {
    // Draw selected objects
    Layer.CanvasObjects.FindAll(o => o.IsSelected).ForEach(o => o.Draw(e.Graphics));
    base.OnPaint(e);
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
    return selectedObjects.FirstOrDefault(o => o.MouseOver(mouseLocation, modifierKeys) != null) ??
      objs.FirstOrDefault(o => o.MouseOver(mouseLocation, modifierKeys) != null);
  }
}
