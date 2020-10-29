using Microsoft.Win32;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TNT.Drawing.DrawingMode;
using TNT.Drawing.Layer;
using TNT.Drawing.Objects;
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

			var layer1 = new CanvasLayer();
			var line = new Line();
			line.AddVertex(new Vertex(300, 100));
			line.AddVertex(new Vertex(100,300));

			layer1.CanvasObjects = new List<CanvasObject>()
			{
				new Square(100,100,100,Color.Blue),
				new Square(500,500,200,Color.Red),
				//new Line(new List<Vertex>(){new Vertex(300,100), new Vertex(100,300)})
				//new Line()
			};

			_CanvasPanel.BackgroundLayer = new CanvasLayer().Also(it =>
			{
				it.CanvasObjects = new List<CanvasObject>() {
					new Square(150,150,200,Color.Green)
				};
			});
			_CanvasPanel.Layers = new List<CanvasLayer>() { layer1 };

			propertyGrid1.SelectedObject = _CanvasPanel.Properties;

			selectToolStripMenuItem.Tag = new SelectMode();
			lineToolStripMenuItem.Tag = new LineMode();

			_CanvasPanel.DrawingMode = selectToolStripMenuItem.Tag as DrawingMode.DrawingMode;
			_CanvasPanel.OnObjectsSelected = (objs) => propertyGrid1.SelectedObject = objs;
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

		private void lineToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			var menuItem = sender as ToolStripMenuItem;
			var mode = menuItem.Tag as DrawingMode.DrawingMode;
			propertyGrid1.SelectedObject = mode.DefaultObject;
			_CanvasPanel.DrawingMode = mode;
		}
	}
}
