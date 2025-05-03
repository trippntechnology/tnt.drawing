using System.Windows.Forms;

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
  public static Feedback Default = new Feedback(Cursors.Default, "");
}
