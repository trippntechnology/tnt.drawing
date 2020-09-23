using System.Drawing;
using System.Windows.Forms;

namespace TNT.Drawing
{
	/// <summary>
	/// Parent <see cref="Panel"/> for the <see cref="Canvas"/>. This was added in order to control scrolling 
	/// reset when focused
	/// </summary>
	public class CanvasPanel : Panel
	{
		/// <summary>
		/// The <see cref="Canvas"/>
		/// </summary>
		public Canvas Canvas { get; set; }

		/// <summary>
		/// Initializes <see cref="CanvasPanel"/>
		/// </summary>
		public CanvasPanel(Control parent)
			: base()
		{
			Parent = parent;
			Width = parent.Width;
			Height = parent.Height;
			Dock = DockStyle.Fill;
			Canvas = new Canvas(this, 0, 0, Width, Height);
			Canvas.BackColor = Color.Yellow;
		}

		/// <summary>
		/// Prevents scroll from resetting when it gets focus
		/// </summary>
		protected override Point ScrollToControl(Control activeControl) => DisplayRectangle.Location;

		/// <summary>
		/// Fits the grid within the parent
		/// </summary>
		public void Fit() => Canvas.Fit();
	}
}
