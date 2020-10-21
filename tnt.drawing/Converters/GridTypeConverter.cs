using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using TNT.Drawing.Layer;

namespace TNT.Drawing.Converters
{
	/// <summary>
	/// Converts <see cref="GridLayer"/> into object that can be viewed in <see cref="PropertyGrid"/> as a 
	/// property of another object
	/// </summary>
	public class GridTypeConverter : TypeConverter
	{
		/// <summary>
		/// Returns the width and height values that are displayed in <see cref="PropertyGrid"/> at the object level
		/// </summary>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(String))
			{
				var grid = value as GridLayer;
				return $"{grid.Width}, {grid.Height}";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// Gets a listing of <see cref="GridLayer"/> properties that can be viewed by the <see cref="PropertyGrid"/>
		/// </summary>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(GridLayer), attributes);//.Sort(new string[] { "PixelPerSegment", "LineColor", "BackgroundColor" });
		}

		/// <summary>
		/// Allows all <see cref="GridLayer"/> properties to be listed in <see cref="PropertyGrid"/>
		/// </summary>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;
	}
}
