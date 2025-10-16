using System.Windows.Forms;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Model;

/// <summary>
/// Represents feedback information, including a cursor and a hint message.
/// </summary>
/// <param name="Cursor">The cursor to display.</param>
/// <param name="Hint">The hint message to show.</param>
public record Feedback(Cursor Cursor, string Hint)
{
  /// <summary>
  /// The default feedback instance with a default cursor and an empty hint.
  /// </summary>
  public static readonly Feedback Default = new Feedback(System.Windows.Forms.Cursors.Default, string.Empty);

  public static readonly Feedback SELECT_DEFAULT = new Feedback(Resources.Cursors.Hand, "Click to select. Ctrl+Click to select multiple objects.");
  public static readonly Feedback SELECT_MULTIPLE = new Feedback(Resources.Cursors.Hand, "Click to toggle selection.");
  public static readonly Feedback SELECT_ADD = new Feedback(Resources.Cursors.Add, "Add.");
  public static readonly Feedback SELECT_REMOVE_POINT = new Feedback(Resources.Cursors.Remove, "Remove.");
  public static readonly Feedback SELECT_MOVE = new Feedback(Resources.Cursors.Drag, "Move.");
  public static readonly Feedback SELECT_ADD_CTRL_POINT = new Feedback(Resources.Cursors.AddControlPoint, "Move.");

  public static readonly Feedback SELECT_DRAG_CTRL_POINT = new Feedback(Resources.Cursors.DragControlPoint, "Move.");
  public static readonly Feedback SELECT_DRAG_POINT = new Feedback(Resources.Cursors.DragPoint, "Move.");
  public static readonly Feedback SELECT_HIDE_CTRL_POINT = new Feedback(Resources.Cursors.HideControlPoint, "Move.");
  public static readonly Feedback SELECT_ADD_SELECTION = new Feedback(Resources.Cursors.AddSelection, "Move.");
  public static readonly Feedback SELECT_REMOVE_SELECTION = new Feedback(Resources.Cursors.RemoveSelection, "Move.");
  public static readonly Feedback SELECT_ROTATE = new Feedback(Resources.Cursors.Rotate, "Move.");


  public static readonly Feedback LINE_MODE_INITIAL_VERTEX = new Feedback(Resources.Cursors.DrawPolyLine, "Click to add initial vertex.");
  public static readonly Feedback LINE_MODE_ADD_VERTEX = new Feedback(Cursors.Default, "Press Shift to contrain to angle. Ctrl or close path to terminate.");
  public static readonly Feedback LINE_MODE_SECOND_VERTEX = new Feedback(Resources.Cursors.DrawPolyLine, "Press Shift to contrain to angle. Ctrl+Click to terminate.");
  public static readonly Feedback LINE_MODE_SECOND_VERTEX_CTRL = new Feedback(Resources.Cursors.DrawPolyLine, "Press Shift to contrain to angle. Click to terminate.");
  public static readonly Feedback LINE_MODE_CLOSED = new Feedback(Resources.Cursors.DrawClosedPolyLine, "");

  public static readonly Feedback RECTANGLE_MODE_INITIAL_VERTEX = new Feedback(Resources.Cursors.DrawRectangle, "Click to add initial vertex.");
  public static readonly Feedback RECTANGLE_MODE_OPPOSITE_VERTEX = new Feedback(Resources.Cursors.DrawRectangle, "Click to add opposite vertex. Shift to constrain to a square.");
  public static readonly Feedback RECTANGLE_MODE_OPPOSITE_VERTEX_SHIFT = new Feedback(Resources.Cursors.DrawRectangle, "Click to add opposite vertex.");

}
