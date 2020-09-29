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
		/// Initializes <see cref="CanvasPanel"/>
		/// </summary>
		public CanvasPanel(Control parent)
				: base()
		{
			Parent = parent;
			Width = parent.Width;
			Height = parent.Height;
			Dock = DockStyle.Fill;
		}

		/// <summary>
		/// Prevents scroll from resetting when it gets focus
		/// </summary>
		protected override Point ScrollToControl(Control activeControl) => DisplayRectangle.Location;
	}
}
