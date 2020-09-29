using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TNT.Drawing
{
	/// <summary>
	/// Base class of classes that back the value using <see cref="_BackingObject"/>.
	/// </summary>
	public abstract class BackedProperties<C> 
	{
		private C _BackingObject;

		/// <summary>
		/// Initilizer
		/// </summary>
		public BackedProperties(C backingObject)
		{
			if (backingObject == null) throw new ArgumentNullException("backingObject can not be null");
			_BackingObject = backingObject;
		}

		/// <summary>
		/// Sets <paramref name="value"/> on the <see cref="_BackingObject"/> with the same name as
		/// <paramref name="propertyName"/>
		/// </summary>
		protected void Set<T>(T value, [CallerMemberName] string propertyName = null)
		{
			Debug.WriteLine($"Set({value}, {propertyName})");
			var canvasPropInfo = _BackingObject.GetType().GetProperty(propertyName);
			canvasPropInfo?.SetValue(_BackingObject, value);
		}

		/// <summary>
		/// Gets the value on the <see cref="_BackingObject"/> with the same name as <paramref name="propertyName"/>
		/// </summary>
		protected T Get<T>([CallerMemberName] string propertyName = null)
		{
			var canvasPropInfo = _BackingObject.GetType().GetProperty(propertyName);
			var value = (T)canvasPropInfo?.GetValue(_BackingObject);
			Debug.WriteLine($"Get({value}, {propertyName})");
			return value;
		}
	}
}
