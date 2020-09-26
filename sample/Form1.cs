using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using TNT.Utilities;

namespace TNT.Drawing.Sample
{
	public partial class Form1 : Form
	{
		private ApplicationRegistry applicationRegistery = null;
		private CanvasPanel _CanvasPanel = null;

		public Form1()
		{
			InitializeComponent();
			applicationRegistery = new ApplicationRegistry(this, Registry.CurrentUser, "Tripp'n Technology", "CenteredDrawing");
			_CanvasPanel = new CanvasPanel(splitContainer1.Panel1);
			_CanvasPanel.Canvas.MouseMove += _MyControl_MouseMove;
			_CanvasPanel.Properties = new CanvasPanelProperties { BackColor = Color.Blue };
			propertyGrid1.SelectedObject = _CanvasPanel.Properties;
		}

		private void _MyControl_MouseMove(object sender, MouseEventArgs e)
		{
			toolStripStatusLabel1.Text = $"{e.X}, {e.Y}";
		}

		private void fitToolStripMenuItem_Click(object sender, System.EventArgs e) => _CanvasPanel.Fit();
	}
}
