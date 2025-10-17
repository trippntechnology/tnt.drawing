using TNT.Drawing.Resource;
using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class Rectangle() : ToolStripItemGroup(Resources.Images.RectangleTool)
{
  public override string Text => "Rectangle";

  public override string ToolTipText => "Rectangle Mode";
}
