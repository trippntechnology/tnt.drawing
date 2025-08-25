using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class Fit() : ToolStripItemGroup(ResourceToImage("Sample.Images.fit_screen_24.png"))
{
  public override string Text => "Fit";

  public override string ToolTipText => "Fit to Screen";
}
