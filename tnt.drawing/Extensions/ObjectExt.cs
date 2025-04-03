using System.Linq;

namespace TNT.Drawing.Extensions;

/// <summary>
/// Provides extension methods for objects.
/// </summary>
public static class ObjectExt
{
  /// <summary>
  /// Copies the properties from the source object to the destination object.
  /// </summary>
  /// <param name="source">The source object.</param>
  /// <param name="destination">The destination object.</param>
  public static void SetProperties(this object source, object destination)
  {
    var sourceProperties = source.GetType().GetProperties();
    var destinationProperties = destination.GetType().GetProperties();
    foreach (var sourceProperty in sourceProperties)
    {
      var destinationProperty = destinationProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
      if (destinationProperty != null && destinationProperty.CanWrite)
      {
        var value = sourceProperty.GetValue(source);
        destinationProperty.SetValue(destination, value);
      }
    }
  }
}
