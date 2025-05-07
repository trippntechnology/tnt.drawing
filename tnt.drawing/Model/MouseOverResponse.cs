using TNT.Drawing.Objects;

namespace TNT.Drawing.Model;

/// <summary>
/// Represents the response when the mouse is over a canvas object.
/// </summary>
/// <param name="HitObject">The canvas object that the mouse is currently over, or null if no object is under the mouse.</param>
public record MouseOverResponse(CanvasObject? HitObject = null)
{
  /// <summary>
  /// Provides a default response indicating no object is under the mouse.
  /// </summary>
  public static readonly MouseOverResponse Default = new MouseOverResponse();
}
