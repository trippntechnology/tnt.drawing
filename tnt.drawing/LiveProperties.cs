using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace TNT.Drawing;

/// <summary>
/// Manages properties that can notify when changed
/// </summary>
public abstract class LiveProperties
{
  /// <summary>
  /// Key/Value pairs of backing fields
  /// </summary>
  protected Dictionary<string, object?> _BackingFields = new Dictionary<string, object?>();

  /// <summary>
  /// Called when a property value has changed
  /// </summary>
  [XmlIgnore]
  public Action<string, object?> OnPropertyChanged = (k, v) => { };

  /// <summary>
  /// Sets <paramref name="value"/> on <see cref="_BackingFields"/> with the same name as
  /// <paramref name="propertyName"/>
  /// </summary>
  public void Set<T>(T? value, [CallerMemberName] string propertyName = "")
  {
    var setValue = false;

    if (_BackingFields.TryGetValue(propertyName, out object? currentValue))
    {
      setValue = currentValue?.Equals(value) == false;
    }
    else
    {
      setValue = true;
    }

    if (setValue)
    {
      _BackingFields[propertyName] = value;
      OnPropertyChanged(propertyName, value);
    }
  }

  /// <summary>
  /// Gets the value on <see cref="_BackingFields"/> with the same name as <paramref name="propertyName"/>
  /// </summary>
  public T? Get<T>([CallerMemberName] string propertyName = "")
  {
    var value = default(T);
    if (_BackingFields.ContainsKey(propertyName))
    {
      value = (T?)_BackingFields[propertyName];
    }
    return value;
  }

  /// <summary>
  /// Sets the <paramref name="propertyName"/> value on the <paramref name="obj"/> to
  /// <paramref name="value"/>
  /// </summary>
  public static void Set(object obj, string propertyName, object? value)
  {
    var propInfo = obj.GetType().GetProperty(propertyName);
    propInfo?.SetValue(obj, value);
  }

  /// <summary>
  /// Sets the all properties that have a common name between <paramref name="source"/> and 
  /// <paramref name="destination"/>
  /// </summary>
  public static void SetAll(object source, object destination)
  {
    var sourceProps = source.GetType().GetProperties().ToList();
    sourceProps.ForEach(propInfo =>
    {
      var propName = propInfo.Name;
      var value = propInfo.GetValue(source);
      Set(destination, propName, value);
    });
  }
}