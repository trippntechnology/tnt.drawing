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
				{"SnapInterval", 10},
				{"SnapToInterval", true},
			};
		}

		/// <summary>
		/// Background color of the <see cref="Canvas"/>
		/// </summary>
		[XmlIgnore]
		[DisplayName("Back Color")]
		public Color BackColor { get { return Get<Color>(); } set { Set(value); } }

		/// <summary>
		/// Serializable for <see cref="BackColor"/>
		/// </summary>
		[Browsable(false)]
		public int _BackColor { get { return BackColor.ToArgb(); } set { BackColor = Color.FromArgb(value); } }

		/// <summary>
		/// </summary>
		[DisplayName("Layer Height")]
		public int LayerHeight { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// </summary>
		[DisplayName("Layer Width")]
		public int LayerWidth { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// ScalePercentage of the <see cref="Canvas"/>
		/// </summary>
		[DisplayName("Scale Percentage")]
		public int ScalePercentage { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// Indicates the snap interval
		/// </summary>
		[DisplayName("Snap Interval")]
		[ReadOnly(true)]
		public int SnapInterval { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// Indicates objects should be snapped to <see cref="SnapInterval"/>
		/// </summary>
		[DisplayName("Snap To Interval")]
		public bool SnapToInterval { get { return Get<bool>(); } set { Set(value); } }
	}
}