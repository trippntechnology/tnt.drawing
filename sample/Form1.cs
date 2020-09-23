using Microsoft.Win32;
using System.Windows.Forms;
using TNT.Drawing;
using TNT.Utilities;

namespace TNT.Drawing.Sample
{
	public partial class Form1 : Form
	{
		private ApplicationRegistry applicationRegistery = null;
		private CanvasPanel _MyControl = null;

		public Form1()
		{
			InitializeComponent();
			applicationRegistery = new ApplicationRegistry(this, Registry.CurrentUser, "Tripp'n Technology", "CenteredDrawing");
			_MyControl = new CanvasPanel(splitContainer1.Panel1);
			_MyControl.MouseMove += _MyControl_MouseMove;
			propertyGrid1.SelectedObject = _MyControl.Canvas;
		}

		private void _MyControl_MouseMove(object sender, MouseEventArgs e)
		{
			toolStripStatusLabel1.Text = $"{e.X}, {e.Y}";
		}

		private void fitToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			_MyControl.Fit();
		}
	}
}
