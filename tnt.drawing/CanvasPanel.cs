using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TNT.Drawing
{
	/// <summary>
	/// Parent <see cref="Panel"/> for the <see cref="Canvas"/>. This was added in order to control scrolling 
	/// reset when focused
	/// </summary>
	public class CanvasPanel : Panel
	{
		private Properties _Properties = null;

		/// <summary>
		/// The <see cref="Canvas"/>
		/// </summary>
		public Canvas Canvas { get; set; }

		/// <summary>
		/// Changable properties associated with a <see cref="CanvasPanel"/> and <see cref="Canvas"/>
		/// </summary>
		public Properties Properties
		{
			get { return _Properties; }
			set
			{
				_Properties = value;
				_Properties.PropertyChanged += OnPropertyChanged;
				_Properties.GetType().GetProperties().ToList().ForEach(p =>
				{
					OnPropertyChanged(this, new PropertyChangedEventArgs(p.Name));
				});
			}
		}

		/// <summary>
		/// Sets the <see cref="Properties"/> value on the corresponding <see cref="Canvas"/> property
		/// </summary>
		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var changedPropInfo = Properties.GetType().GetProperty(e.PropertyName);
			var changingPropInfo = Canvas.GetType().GetProperty(e.PropertyName);
			changingPropInfo?.SetValue(Canvas, changedPropInfo.GetValue(Properties));
		}

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
