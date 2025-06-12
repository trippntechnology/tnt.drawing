using System.Drawing;
using System.Windows.Forms;

namespace TNT.Drawing.Interface;

/// <summary>
/// Defines an object that can be rotated by a specified angle.
/// </summary>
public interface IRotatable
{
  /// <summary>
  /// Rotates the object by the specified angle in degrees around its centroid.
  /// </summary>
  /// <param name="angle">The rotation angle in degrees to apply.</param>
  /// <param name="modifierKeys">Modifier keys pressed during the rotation operation.</param>
  void RotateBy(double angle, Keys modifierKeys);

  /// <summary>
  /// Returns the centroid (center of mass) of the object in canvas coordinates.
  /// This point will be used as the center of rotation.
  /// Returns null if the centroid is not defined for this object.
  /// </summary>
  Point? GetCentroid();
}