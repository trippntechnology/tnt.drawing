using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class Line() : ToolStripItemGroup(ResourceToImage("Sample.Images.line_curve_24.png"))
{
  public override string Text => "Line";

  public override string ToolTipText => "Line Mode";
}
