using TNT.ToolStripItemManager;

namespace Sample.Group;

internal class AlignToGrid() : ToolStripItemGroup(ResourceToImage("Sample.Images.grid_guides_24.png"))
{
  public override string Text => "Align to Grid";

  public override string ToolTipText => "Align vertices to grid intersections";
}
