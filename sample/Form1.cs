using Microsoft.Win32;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TNT.Drawing.DrawingModes;
using TNT.Drawing.Layers;
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

			var line1 = new Line();
			line1.AddVertex(new Vertex(300, 100));
			line1.AddVertex(new Vertex(400, 300));
			line1.AddVertex(new Vertex(500, 100));

			var line2 = new Line();
			line2.AddVertex(new Vertex(500, 100));
			line2.AddVertex(new Vertex(600, 300));
			line2.AddVertex(new Vertex(700, 100));

			var layer1 = new CanvasLayer(_CanvasPanel)
			{
				Name = "Background",
				CanvasObjects = new List<CanvasObject>() { new Square(150, 150, 200, Color.Green) },
				BackgroundColor = Color.White,
			};

			var layer2 = new GridLayer(_CanvasPanel) { Name = "Grid", LineColor = Color.Aqua };

			var layer3 = new CanvasLayer(_CanvasPanel)
			{
				Name = "Object",
				CanvasObjects = new List<CanvasObject>()
				{
					new Square(100,100,100,Color.Blue),
					new Square(500,500,200,Color.Red),
					line1,
					line2,
				}
			};

			_CanvasPanel.Layers = new List<CanvasLayer>() { layer1, layer2, layer3 };

			_CanvasPanel.Layers.ForEach(layer =>
			{
				layerToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(layer.ToString()).Also(it =>
				{
					it.Tag = layer;
					it.Click += (sender, e) => { propertyGrid1.SelectedObject = (sender as ToolStripMenuItem).Tag; };
				}));
			});

			propertyGrid1.SelectedObject = _CanvasPanel.Properties;

			selectToolStripMenuItem.Tag = new DrawingModes.SelectMode(layer3);
			lineToolStripMenuItem.Tag = new LineMode(layer3);

			_CanvasPanel.DrawingMode = selectToolStripMenuItem.Tag as DrawingMode;
			_CanvasPanel.OnSelected = (objs) =>
			{
				try
				{
					propertyGrid1.SelectedObjects = objs.ToArray();
				}
				catch (System.Exception)
				{
				}
			};
		}

		//private void _MyControl_MouseMove(object sender, MouseEventArgs e) => toolStripStatusLabel1.Text = $"{e.X}, {e.Y}";

		private void fitToolStripMenuItem_Click(object sender, System.EventArgs e) => _CanvasPanel.Fit();

		private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			using (var sfd = new SaveFileDialog())
			{
				if (sfd.ShowDialog() == DialogResult.OK)
				{
					var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					var assPath = Path.Combine(path, "tnt.drawing.dll");
					var types = Utilities.Utilities.GetTypes(assPath, t => !t.IsAbstract && (t.InheritsFrom(typeof(CanvasObject)) || t.InheritsFrom(typeof(CanvasLayer))));

					var layer = _CanvasPanel.Layers.Find(l => l.Name == "Object");
					var ser = Utilities.Utilities.Serialize(layer, types);
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
					var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					var assPath = Path.Combine(path, "tnt.drawing.dll");
					var types = Utilities.Utilities.GetTypes(assPath, t => !t.IsAbstract && (t.InheritsFrom(typeof(CanvasObject)) || t.InheritsFrom(typeof(CanvasLayer))));

					var layer = _CanvasPanel.Layers.Find(l => l.Name == "Object");
					_CanvasPanel.Layers.Remove(layer);
					var ser = File.ReadAllText(ofd.FileName);
					layer = Utilities.Utilities.Deserialize<CanvasLayer>(ser, types);
					_CanvasPanel.Layers.Add(layer);
					//_CanvasPanel.Properties = Utilities.Utilities.Deserialize<CanvasProperties>(ser, new System.Type[] { });
					propertyGrid1.SelectedObject = _CanvasPanel.Properties;
				}
			}
		}

		private void lineToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			var menuItem = sender as ToolStripMenuItem;
			var mode = menuItem.Tag as DrawingMode;
			propertyGrid1.SelectedObject = mode.DefaultObject;
			_CanvasPanel.DrawingMode?.Reset();
			_CanvasPanel.DrawingMode = mode;
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) => _CanvasPanel.Refresh(_CanvasPanel?.DrawingMode?.Layer);
	}
}
