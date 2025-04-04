using System.Linq;

namespace TNT.Drawing.Extensions;

/// <summary>
/// Provides extension methods for objects.
/// </summary>
public static class ObjectExt
{
  /// <summary>
  /// Sets the value of a specified property on an object.
  /// </summary>
  /// <param name="obj">The object whose property value will be set.</param>
  /// <param name="name">The name of the property to set.</param>
  /// <param name="value">The value to set to the property.</param>
  public static void SetProperty(this object obj, string name, object? value)
  {
    var destinationProperties = obj.GetType().GetProperties();

    var destinationProperty = destinationProperties.FirstOrDefault(p => p.Name == name);
    if (destinationProperty != null && destinationProperty.CanWrite)
    {
      destinationProperty.SetValue(obj, value);
    }
  }

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
      destination.SetProperty(sourceProperty.Name, sourceProperty.GetValue(source, null));
    }
  }
}
