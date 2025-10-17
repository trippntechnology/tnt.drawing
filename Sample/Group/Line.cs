using TNT.Drawing.Resource;
using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class Line() : ToolStripItemGroup(Resources.Images.PolylineTool)
{
  public override string Text => "Line";

  public override string ToolTipText => "Line Mode";
}
