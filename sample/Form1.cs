﻿using Microsoft.Win32;
using TNT.Commons;
using TNT.Drawing.DrawingModes;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;
using TNT.Utilities;


namespace TNT.Drawing.Sample;

public partial class Form1 : Form
{
  private ApplicationRegistry? applicationRegistery = null;
  private Canvas _Canvas;

  public Form1()
  {
    InitializeComponent();
    applicationRegistery = new ApplicationRegistry(this, Registry.CurrentUser, "Tripp'n Technology", "CenteredDrawing");
    _Canvas = new Canvas(splitContainer1.Panel1);
    _Canvas.Properties = new CanvasProperties(); ;

    var line1 = new Line();
    line1.AddVertex(new Vertex(300, 100));
    line1.AddVertex(new Vertex(400, 300));
    line1.AddVertex(new Vertex(500, 100));

    var line2 = new Line();
    line2.AddVertex(new Vertex(500, 100));
    line2.AddVertex(new Vertex(600, 300));
    line2.AddVertex(new Vertex(700, 100));

    var layer1 = new CanvasLayer(_Canvas)
    {
      Name = "Background",
      CanvasObjects = new List<CanvasObject>() { new Square(150, 150, 200, Color.Green) },
      BackgroundColor = Color.White,
    };

    var layer2 = new GridLayer(_Canvas) { Name = "Grid", LineColor = Color.Aqua };

    var layer3 = new CanvasLayer(_Canvas)
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

    _Canvas.Layers = new List<CanvasLayer>() { layer1, layer2, layer3 };

    _Canvas.Layers.ForEach(layer =>
    {
      layerToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(layer.ToString()).Also(it =>
      {
        it.Tag = layer;
        it.Click += (sender, e) => { propertyGrid1.SelectedObject = (sender as ToolStripMenuItem)?.Tag; };
      }));
    });

    propertyGrid1.SelectedObject = _Canvas.Properties;

    selectToolStripMenuItem.Tag = new DrawingModes.SelectMode(layer3);
    lineToolStripMenuItem.Tag = new LineMode(layer3);

    _Canvas.DrawingMode = selectToolStripMenuItem.Tag as DrawingMode;
    _Canvas.OnSelected = (objs) =>
    {
      try
      {
        propertyGrid1.SelectedObjects = objs.ToArray();
      }
      catch (System.Exception)
      {
      }
    };

    _Canvas.OnFeedbackChanged = (cursor, hint) =>
    {
      Cursor = cursor;
      toolStripStatusLabel1.Text = hint;
    };
  }

  private void FitToolStripMenuItem_Click(object sender, System.EventArgs e) => _Canvas?.Fit();

  private void SaveToolStripMenuItem_Click(object sender, System.EventArgs e)
  {
    using (var sfd = new System.Windows.Forms.SaveFileDialog())
    {
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //var assPath = Path.Combine(path, "tnt.drawing.dll");
        //var types = Utilities.Utilities.GetTypes(assPath, t => !t.IsAbstract && (t.InheritsFrom(typeof(CanvasObject)) || t.InheritsFrom(typeof(CanvasLayer))));

        //var layer = _Canvas.Layers.Find(l => l.Name == "Object");
        //var ser = Utilities.Utilities.Serialize(layer, types);
        //File.WriteAllText(sfd.FileName, ser);
      }
    }
  }

  private void OpenToolStripMenuItem_Click(object sender, System.EventArgs e)
  {
    using (var ofd = new System.Windows.Forms.OpenFileDialog())
    {
      if (ofd.ShowDialog() == DialogResult.OK)
      {
        //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //var assPath = Path.Combine(path, "tnt.drawing.dll");
        //var types = Utilities.Utilities.GetTypes(assPath, t => !t.IsAbstract && (t.InheritsFrom(typeof(CanvasObject)) || t.InheritsFrom(typeof(CanvasLayer))));

        //var layer = _Canvas.Layers.Find(l => l.Name == "Object");
        //_Canvas.Layers.Remove(layer);
        //var ser = File.ReadAllText(ofd.FileName);
        //layer = Utilities.Utilities.Deserialize<CanvasLayer>(ser, types);
        //_Canvas.Layers.Add(layer);
        ////_CanvasPanel.Properties = Utilities.Utilities.Deserialize<CanvasProperties>(ser, new System.Type[] { });
        //propertyGrid1.SelectedObject = _Canvas.Properties;
      }
    }
  }

  private void LineToolStripMenuItem_Click(object sender, System.EventArgs e)
  {
    if (sender is ToolStripMenuItem menuItem)
    {
      if (menuItem.Tag is DrawingMode mode)
      {
        propertyGrid1.SelectedObject = mode.DefaultObject;
        _Canvas.DrawingMode?.Reset();
        _Canvas.DrawingMode = mode;
      }
    }
  }

  private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) => _Canvas.DrawingMode?.Layer?.Also(layer => _Canvas.Refresh(layer));
  private void AlignToolStripMenuItem_Click(object sender, System.EventArgs e) => _Canvas.AlignToInterval();
}
