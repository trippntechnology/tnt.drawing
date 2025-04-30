using TNT.Drawing.Objects;

namespace TNT.Drawing.Model;

public record MouseDownResponse(CanvasObject? HitObject = null, CanvasObject? ChildHitObject = null, bool AllowMove = true);
