using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class OpenFile() : ToolStripItemGroup(ResourceToImage("Sample.Images.file_open_24.png"))
{
  public override string Text => "Open";

  public override string ToolTipText => "Open File";
}
