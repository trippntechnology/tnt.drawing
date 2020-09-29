using System.Drawing;

namespace TNT.Drawing
{
	/// <summary>
	/// Properties that can be changed in a <see cref="CanvasPanel"/>
	/// </summary>
	public class CanvasProperties : BackedProperties<Canvas>
	{
		/// <summary>
		/// Initializer
		/// </summary>
		public CanvasProperties(Canvas canvas) : base(canvas)
		{
		}

		/// <summary>
		/// Background color of the <see cref="Canvas"/>
		/// </summary>
		public Color BackColor { get { return Get<Color>(); } set { Set(value); } }

		/// <summary>
		/// BackgroundColor of the <see cref="Canvas.Grid"/>
		/// </summary>
		public Color BackgroundColor { get { return Get<Color>(); } set { Set(value); } }

		/// <summary>
		/// Height of the <see cref="Canvas.Grid"/>
		/// </summary>
		public int GridHeight { get { return Get<int>(); } set { Set(value); } }

		/// <summary>
		/// Line color of the <see cref="Canvas.Grid"/>
		/// </summary>
		public Color GridLineColor { get { return Get<Color>(); } set { Set(value); } }

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