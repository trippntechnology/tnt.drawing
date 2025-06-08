using Microsoft.Win32;
using TNT.Commons;
using TNT.Drawing;
using TNT.Drawing.DrawingModes;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;
using TNT.Utilities;


namespace Sample;

public partial class Form1 : Form
{
  private ApplicationRegistry? applicationRegistery = null;
  private readonly Canvas canvas;

  public Form1()
  {
    InitializeComponent();
    applicationRegistery = new ApplicationRegistry(this, Registry.CurrentUser, "Tripp'n Technology", "CenteredDrawing");
    canvas = new Canvas(splitContainer1.Panel1);
    canvas.Properties = new CanvasProperties(); ;

    var line1 = new BezierPath();
    line1.AddVertex(new Vertex(300, 100));
    line1.AddVertex(new Vertex(400, 300));
    line1.AddVertex(new Vertex(500, 100));

    var line2 = new BezierPath();
    line2.AddVertex(new Vertex(500, 100));
    line2.AddVertex(new Vertex(600, 300));
    line2.AddVertex(new Vertex(700, 100));

    var backgroundLayer = new CanvasLayer(canvas)
    {
      Name = "Background",
      CanvasObjects = new List<CanvasObject>() { CreateSquarePath(150, 150, 50, Color.Green) },
      BackgroundColor = Color.White,
    };

    var gridLayer = new GridLayer(canvas) { Name = "Grid", LineColor = Color.Aqua };

    // Demonstrate ClosedBezierPath
    var closedBezier = new BezierPath();
    closedBezier.AddVertex(new Vertex(200, 400));
    closedBezier.AddVertex(new Vertex(300, 600));
    closedBezier.AddVertex(new Vertex(500, 600));
    closedBezier.AddVertex(new Vertex(600, 400));
    closedBezier.AddVertex(new Vertex(200, 400));
    closedBezier.FillColor = Color.FromArgb(128, Color.Orange);

    var objectsLayer = new CanvasLayer(canvas)
    {
      Name = "Object",
      CanvasObjects = new List<CanvasObject>()
      {
        CreateSquarePath(100,100,100, Color.Blue),
        CreateSquarePath(500,500,200, Color.Red),
        line1,
        line2,
        closedBezier, // Add ClosedBezierPath to the sample
      }
    };

    canvas.Layers = new List<CanvasLayer>() { backgroundLayer, gridLayer, objectsLayer };

    canvas.Layers.ForEach(layer =>
    {
      layerToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(layer.ToString()).Also(it =>
      {
        it.Tag = layer;
        it.Click += (sender, e) => { propertyGrid1.SelectedObject = (sender as ToolStripMenuItem)?.Tag; };
      }));
    });

    propertyGrid1.SelectedObject = canvas.Properties;

    var modes = new List<DrawingMode> {
      new SelectMode(canvas,      objectsLayer),
      new LineMode(canvas, objectsLayer, new BezierPath()),
      new BezierPathMode(canvas, objectsLayer, new BezierPath()),
      new SquareMode(canvas, objectsLayer, new BezierPath()),
    };

    modes.ForEach(mode =>
    {
      var menuItem = new ToolStripMenuItem(mode.ToString());
      menuItem.Tag = mode;
      menuItem.Click += LineToolStripMenuItem_Click;
      modeToolStripMenuItem.DropDownItems.Add(menuItem);
    });

    canvas.DrawingMode = modes.First();
    canvas.OnSelected = (objs) =>
    {
      try
      {
        propertyGrid1.SelectedObjects = objs.ToArray();
      }
      catch (System.Exception)
      {
      }
    };

    canvas.OnFeedbackChanged = (feedback) =>
    {
      Cursor = feedback.Cursor;
      toolStripStatusLabel1.Text = feedback.Hint;
    };
  }

  private BezierPath CreateSquarePath(int x, int y, int size, Color fillColor)
  {
    var path = new BezierPath
    {
      FillColor = fillColor,
    };
    path.AddVertex(new Vertex(x, y));
    path.AddVertex(new Vertex(x + size, y));
    path.AddVertex(new Vertex(x + size, y + size));
    path.AddVertex(new Vertex(x, y + size));
    path.AddVertex(new Vertex(x, y)); // Close the square

    return path;
  }

  private void FitToolStripMenuItem_Click(object sender, System.EventArgs e) => canvas?.Fit();

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

  private void LineToolStripMenuItem_Click(object? sender, System.EventArgs e)
  {
    if (sender is ToolStripMenuItem menuItem)
    {
      if (menuItem.Tag is DrawingMode mode)
      {
        propertyGrid1.SelectedObject = mode.DefaultObject;
        canvas.DrawingMode.Reset();
        canvas.DrawingMode = mode;
      }
    }
  }

  private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) => canvas.DrawingMode.Layer.Also(layer => canvas.Refresh(layer));
  private void AlignToolStripMenuItem_Click(object sender, System.EventArgs e) => canvas.AlignToInterval();
}
