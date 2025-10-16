using TNT.Drawing.Resource;
using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class Line() : ToolStripItemGroup(Resources.Images.Polyline)
{
  public override string Text => "Line";

  public override string ToolTipText => "Line Mode";
}
