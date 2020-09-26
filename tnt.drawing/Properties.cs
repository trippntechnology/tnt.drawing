using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TNT.Drawing
{
	/// <summary>
	/// Class that represents properties.
	/// </summary>
	public abstract class Properties : INotifyPropertyChanged
	{
		/// <summary>
		/// Called when the value of a property is set using <see cref="Set{T}(ref T, T, string)"/>
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Set the <paramref name="backingField"/> of a property and invokes <see cref="PropertyChanged"/>
		/// </summary>
		protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
		{
			// Check if the value and backing field are actualy different
			if (EqualityComparer<T>.Default.Equals(backingField, value))
			{
				return false;
			}

			// Setting the backing field and the RaisePropertyChanged
			backingField = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
