using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace TNT.Drawing
{
	/// <summary>
	/// Properties that can be changed in a <see cref="CanvasPanel"/>
	/// </summary>
	public class CanvasProperties : LiveProperties
	{
		/// <summary>
		/// Initializes default values
		/// </summary>
		public CanvasProperties()
		{
			_BackingFields = new Dictionary<string, object>
			{
				{"BackColor", Color.Blue},
				{"LayerHeight", 768},
				{"LayerWidth", 1024},
				{"ScalePercentage", 100},
			};
		}

		/// <summary>
		/// Background color of the <see cref="Canvas"/>
		/// </summary>
		[XmlIgnore]
		public Color BackColor { get { return Get<Color>(); } set { Set(value); } }

		/// <summary>
		/// Serializable for <see cref="BackColor"/>
		/// </summary>
		[Browsable(false)]
		public int _BackColor { get { return BackColor.ToArgb(); } set { BackColor = Color.FromArgb(value); } }

		/// <summary>
		/// </summary>
		public int LayerHeight { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// </summary>
		public int LayerWidth { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// ScalePercentage of the <see cref="Canvas"/>
		/// </summary>
		public int ScalePercentage { get { return Get<int>(); } set { Set(value); } }
	}
}