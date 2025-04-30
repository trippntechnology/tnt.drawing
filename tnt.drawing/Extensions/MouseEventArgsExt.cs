using System.Windows.Forms;

namespace TNT.Drawing.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="MouseEventArgs"/> class.
/// </summary>
public static class MouseEventArgsExt
{
  /// <summary>
  /// Determines whether any mouse button is currently pressed.
  /// </summary>
  /// <param name="e">The <see cref="MouseEventArgs"/> instance to check.</param>
  /// <returns><c>true</c> if any mouse button is pressed; otherwise, <c>false</c>.</returns>
  public static bool HasButtonDown(this MouseEventArgs e) => e.Button == MouseButtons.Left || e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle;
}
