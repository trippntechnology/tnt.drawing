using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class BringToFront() : ToolStripItemGroup(ResourceToImage("Sample.Images.flip_to_front_24.png"))
{
  public override string Text => "Bring to Front";

  public override string ToolTipText => "Bring selected object to the front";
}
