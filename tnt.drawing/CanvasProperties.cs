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
				{"BackgroundColor", Color.White},
				{"GridHeight", 768},
				{"GridLineColor", Color.Aqua},
				{"GridWidth", 1024},
				{"PixelPerGridLines", 10},
				{"ScalePercentage", 100},
				{"ShowGrid", true},
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
		/// BackgroundColor of the <see cref="Canvas.Grid"/>
		/// </summary>
		[XmlIgnore]
		public Color BackgroundColor { get { return Get<Color>(); } set { Set(value); } }

		/// <summary>
		/// Serializable for <see cref="BackgroundColor"/>
		/// </summary>
		[Browsable(false)]
		public int _BackgroundColor { get { return BackgroundColor.ToArgb(); } set { BackgroundColor = Color.FromArgb(value); } }

		/// <summary>
		/// Height of the <see cref="Canvas.Grid"/>
		/// </summary>
		public int GridHeight { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// Line color of the <see cref="Canvas.Grid"/>
		/// </summary>
		[XmlIgnore]
		public Color GridLineColor { get { return Get<Color>(); } set { Set(value); } }

		/// <summary>
		/// Serializable for <see cref="GridLineColor"/>
		/// </summary>
		[Browsable(false)]
		public int _GridLineColor { get { return GridLineColor.ToArgb(); } set { GridLineColor = Color.FromArgb(value); } }

		/// <summary>
		/// Width of the <see cref="Canvas.Grid"/>
		/// </summary>
		public int GridWidth { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// Pixels between the lines on the <see cref="Canvas.Grid"/>
		/// </summary>
		public int PixelPerGridLines { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// ScalePercentage of the <see cref="Canvas"/>
		/// </summary>
		public int ScalePercentage { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// ShowGrid of the <see cref="Canvas"/>
		/// </summary>
		public bool ShowGrid { get { return Get<bool>(); } set { Set(value); } }
	}
}