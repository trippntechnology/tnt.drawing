using System.Windows.Forms;

namespace TNT.Drawing.Extentions;

internal static class MouseEventArgsExt
{
  public static bool HasButtonDown(this MouseEventArgs e) => e.Button == MouseButtons.Left || e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle;
}
