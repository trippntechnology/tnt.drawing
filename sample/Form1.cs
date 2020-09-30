using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TNT.Utilities;

namespace TNT.Drawing.Sample
{
	public partial class Form1 : Form
	{
		private ApplicationRegistry applicationRegistery = null;
		private Canvas _CanvasPanel = null;

		public Form1()
		{
			InitializeComponent();
			applicationRegistery = new ApplicationRegistry(this, Registry.CurrentUser, "Tripp'n Technology", "CenteredDrawing");
			_CanvasPanel = new Canvas(splitContainer1.Panel1);
			_CanvasPanel.Properties = new CanvasProperties(); ;
			propertyGrid1.SelectedObject = _CanvasPanel.Properties;
		}

		//private void _MyControl_MouseMove(object sender, MouseEventArgs e) => toolStripStatusLabel1.Text = $"{e.X}, {e.Y}";

		private void fitToolStripMenuItem_Click(object sender, System.EventArgs e) => _CanvasPanel.Fit();

		private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			using (var sfd = new SaveFileDialog())
			{
				if (sfd.ShowDialog() == DialogResult.OK)
				{
					var ser = Utilities.Utilities.Serialize(_CanvasPanel.Properties, new System.Type[] { });
					var foo = 0;
					File.WriteAllText(sfd.FileName, ser);
				}
			}
		}

		private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					var ser = File.ReadAllText(ofd.FileName);
					_CanvasPanel.Properties = Utilities.Utilities.Deserialize<CanvasProperties>(ser, new System.Type[] { });
					propertyGrid1.SelectedObject = _CanvasPanel.Properties;
				}
			}
		}
	}
}
