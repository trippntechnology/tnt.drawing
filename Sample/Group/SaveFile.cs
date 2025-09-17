using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class SaveFile() : ToolStripItemGroup(ResourceToImage("Sample.Images.file_save_24.png"))
{
  public override string Text => "Save";

  public override string ToolTipText => "Save Drawing";
}
