using System.Windows.Forms;

namespace TNT.Drawing.Extensions;

public static class KeysExt
{
  /// <summary>
  /// Checks if the specified Keys value contains one or more Keys values.
  /// </summary>
  /// <param name="keys">The Keys value to check.</param>
  /// <param name="keysToCheck">One or more Keys values to check for.</param>
  /// <returns>True if the Keys value contains ALL of the specified Keys values; otherwise, false.</returns>
  public static bool ContainsAll(this Keys keys, params Keys[] keysToCheck)
  {
    foreach (var key in keysToCheck)
    {
      if ((keys & key) != key)
      {
        return false;
      }
    }
    return true;
  }

  /// <summary>
  /// Checks if the specified Keys value contains any of the specified Keys values.
  /// </summary>
  /// <param name="keys">The Keys value to check.</param>
  /// <param name="keysToCheck">One or more Keys values to check for.</param>
  /// <returns>True if the Keys value contains ANY of the specified Keys values; otherwise, false.</returns>
  public static bool ContainsAny(this Keys keys, params Keys[] keysToCheck)
  {
    foreach (var key in keysToCheck)
    {
      if ((keys & key) == key)
      {
        return true;
      }
    }
    return false;
  }
}
