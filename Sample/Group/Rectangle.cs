using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class Rectangle() : ToolStripItemGroup(ResourceToImage("Sample.Images.rectangle_24.png"))
{
  public override string Text => "Rectangle";

  public override string ToolTipText => "Rectangle Mode";
}
