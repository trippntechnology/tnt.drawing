using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TNT.Drawing
{
  /// <summary>
  /// Encapsolates a <see cref="Dictionary{TKey, TValue}"/> representing property name and values
  /// </summary>
  public class BackingFields
  {
    /// <summary>
    /// Called when a value within <see cref="BackingFields"/> changes
    /// </summary>
    public Action<string, object?> OnFieldChanged = (_, __) => { };

    /// <summary>
    /// Represents property names and values
    /// </summary>
    protected Dictionary<string, object> _BackingFields = new Dictionary<string, object>();

    /// <summary>
    /// Default constructor
    /// </summary>
    public BackingFields() { }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public BackingFields(BackingFields backingFields)
    {
      backingFields._BackingFields.Keys.ToList().ForEach(key => _BackingFields[key] = backingFields._BackingFields[key]);
    }

    /// <summary>
    /// Sets the property <paramref name="value"/> associated with the <paramref name="propertyName"/>
    /// </summary>
    public void Set<T>(T value, [CallerMemberName] string propertyName = "")
    {
      var setValue = false;

      if (_BackingFields.TryGetValue(propertyName, out object? currentValue))
      {
        setValue = !currentValue.Equals(value);
      }
      else
      {
        setValue = true;
      }

      if (setValue)
      {
        if (value == null)
        {
          _BackingFields.Remove(propertyName);
        }
        else
        {
          _BackingFields[propertyName] = value;
        }
        OnFieldChanged(propertyName, value);
      }
    }

    /// <summary>
    /// Gets the value associated with the <paramref name="propertyName"/> if exists, otherwise, returns <paramref name="defaultValue"/>
    /// </summary>
    public T? Get<T>(T? defaultValue = default, [CallerMemberName] string propertyName = "")
    {
      var value = defaultValue;
      if (_BackingFields.ContainsKey(propertyName))
      {
        value = (T)_BackingFields[propertyName];
      }
      return value;
    }
  }
}
