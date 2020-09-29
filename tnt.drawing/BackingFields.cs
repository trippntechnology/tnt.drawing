using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TNT.Drawing
{
	/// <summary>
	/// A <see cref="Dictionary{TKey, TValue}"/> that represents the backing fields for
	/// properties.
	/// </summary>
	public class BackingFields : Dictionary<string, object>
	{
		/// <summary>
		/// Sets a <paramref name="value"/> on the <see cref="Dictionary{TKey, TValue}"/> entry
		/// where the key is the <paramref name="propertyName"/>. <see cref="CallerMemberNameAttribute"/>
		/// automatically populates the default value for <paramref name="propertyName"/>.
		/// </summary>
		public void Set<T>(T value, [CallerMemberName] string propertyName = null)
		{
			Debug.WriteLine($"BackingFields.Set({value}, {propertyName})");
			this[propertyName] = value;
		}

		/// <summary>
		/// Gets the value associated with the <see cref="Dictionary{TKey, TValue}"/> key represented
		/// by <paramref name="propertyName"/>
		/// </summary>
		public T Get<T>([CallerMemberName] string propertyName = null)
		{
			var value = (T)this[propertyName];
			Debug.WriteLine($"BackingFields.Get({value}, {propertyName})");
			return value;
		}
	}
}
