using TNT.Drawing.Resource;
using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class Select() : ToolStripItemGroup(Resources.Images.SelectorTool)
{
  public override string Text => "Select";

  public override string ToolTipText => "Select Mode";
}
