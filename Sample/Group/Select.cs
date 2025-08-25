using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class Select() : ToolStripItemGroup(ResourceToImage("Sample.Images.arrow_selector_tool_24.png"))
{
  public override string Text => "Select";

  public override string ToolTipText => "Select Mode";
}
