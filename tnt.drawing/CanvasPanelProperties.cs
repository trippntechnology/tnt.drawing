using System.Drawing;

namespace TNT.Drawing
{
	/// <summary>
	/// Properties that can be changed in a <see cref="CanvasPanel"/>
	/// </summary>
	public class CanvasPanelProperties : Properties
	{
		private Color _BackColor;
		private Color _BackgroundColor = Color.White;
		private int _GridHeight = 768;
		private bool _ShowGrid = true;
		private int _ScalePercentage = 100;
		private int _GridWidth = 1024;
		private Color _GridLineColor = Color.Aqua;
		private int _PixelPerGirdLines = 10;

		/// <summary>
		/// Background color of the <see cref="Canvas"/>
		/// </summary>
		public Color BackColor { get { return _BackColor; } set { Set(ref _BackColor, value); } }

		/// <summary>
		/// ScalePercentage of the <see cref="Canvas"/>
		/// </summary>
		public int ScalePercentage { get { return _ScalePercentage; } set { Set(ref _ScalePercentage, value); } }

		/// <summary>
		/// BackgroundColor of the <see cref="Canvas.Grid"/>
		/// </summary>
		public Color BackgroundColor { get { return _BackgroundColor; } set { Set(ref _BackgroundColor, value); } }

		/// <summary>
		/// ShowGrid of the <see cref="Canvas"/>
		/// </summary>
		public bool ShowGrid { get { return _ShowGrid; } set { Set(ref _ShowGrid, value); } }

		/// <summary>
		/// Height of the <see cref="Canvas.Grid"/>
		/// </summary>
		public int GridHeight { get { return _GridHeight; } set { Set(ref _GridHeight, value); } }

		/// <summary>
		/// Width of the <see cref="Canvas.Grid"/>
		/// </summary>
		public int GridWidth { get { return _GridWidth; } set { Set(ref _GridWidth, value); } }

		/// <summary>
		/// Line color of the <see cref="Canvas.Grid"/>
		/// </summary>
		public Color GridLineColor { get { return _GridLineColor; } set { Set(ref _GridLineColor, value); } }

		/// <summary>
		/// Pixels between the lines on the <see cref="Canvas.Grid"/>
		/// </summary>
		public int PixelPerGridLines { get { return _PixelPerGirdLines; } set { Set(ref _PixelPerGirdLines, value); } }
	}
}
