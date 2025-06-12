using TNT.Drawing.Objects;

namespace TNT.Drawing.Model;

/// <summary>
/// Represents the response to a mouse down event on a canvas object.
/// </summary>
/// <param name="HitObject">The canvas object that was hit by the mouse down event, if any.</param>
/// <param name="InnerHitObject">The inner (child or nested) canvas object that was hit by the mouse down event, if any.</param>
/// <param name="AllowMove">Indicates whether the object can be moved as a result of the mouse down event.</param>
public record MouseDownResponse(CanvasObject? HitObject = null, CanvasObject? InnerHitObject = null, bool AllowMove = true)
{
  /// <summary>
  /// Provides a default response indicating no object was hit.
  /// </summary>
  public static readonly MouseDownResponse Default = new MouseDownResponse();
}
