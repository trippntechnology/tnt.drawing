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

  public static readonly Feedback LINE_MODE_DEFAULT = new Feedback(Resources.Cursors.DrawLine, "Click to add vertex.");
  public static readonly Feedback LINEMODE_MANY_VERTEX = new Feedback(Cursors.Default, "Press Shift to contrain to angle. Ctrl or close path to terminate.");
  public static readonly Feedback LINEMODE_ONE_VERTEX = new Feedback(Cursors.Default, "Press Shift to contrain to angle. Ctrl to terminate.");

  public static readonly Feedback RECTANGLE_MODE_INITIAL_VERTEX = new Feedback(Resources.Cursors.DrawRectangle, "Click to add initial vertex.");
  public static readonly Feedback RECTANGLE_MODE_OPPOSITE_VERTEX = new Feedback(Resources.Cursors.DrawRectangle, "Click to add opposite vertex. Shift to constrain to a square.");
  public static readonly Feedback RECTANGLE_MODE_OPPOSITE_VERTEX_SHIFT = new Feedback(Resources.Cursors.DrawRectangle, "Click to add opposite vertex.");

}
