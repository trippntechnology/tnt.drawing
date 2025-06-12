using TNT.Drawing.Objects;

namespace TNT.Drawing.Model;

/// <summary>
/// Represents the response to a mouse up event on a canvas object.
/// </summary>
/// <param name="HitObject">The canvas object that was under the mouse up event, if any.</param>
/// <param name="InnerHitObject">The inner (child or nested) canvas object that was under the mouse up event, if any.</param>
/// <param name="AllowMove">Indicates whether the object can be moved as a result of the mouse up event.</param>
public record MouseUpResponse(CanvasObject? HitObject = null, CanvasObject? InnerHitObject = null, bool AllowMove = true)
{
  /// <summary>
  /// Provides a default response indicating no object was under the mouse up event.
  /// </summary>
  public static readonly MouseUpResponse Default = new MouseUpResponse();
}
