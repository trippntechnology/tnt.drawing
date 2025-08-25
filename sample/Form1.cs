using Microsoft.Win32;
using Sample.Group;
using TNT.Commons;
using TNT.Drawing;
using TNT.Drawing.DrawingModes;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;
using TNT.ToolStripItemManager;
using TNT.Utilities;


namespace Sample;

public partial class Form1 : Form
{
  private ApplicationRegistry? applicationRegistery = null;
  private readonly Canvas canvas;
  private ToolStripItemCheckboxGroupManager? ModeGroupManager;
  private ToolStripItemGroupManager? GroupManager;

  public Form1()
  {
    InitializeComponent();

    applicationRegistery = new ApplicationRegistry(this, Registry.CurrentUser, "Tripp'n Technology", "CenteredDrawing");
    canvas = new Canvas(toolStripContainer1.ContentPanel);
    canvas.Properties = new CanvasProperties(); ;

    var backgroundLayer = new CanvasLayer(canvas)
    {
      Name = "Background",
      CanvasObjects = new List<CanvasObject>() { GetSquarePath(150, 150, 50, Color.Green) },
      BackgroundColor = Color.White,
    };

    var gridLayer = new GridLayer(canvas) { Name = "Grid", LineColor = Color.Aqua };

    var objectsLayer = new CanvasLayer(canvas)
    {
      Name = "Object",
      CanvasObjects = new List<CanvasObject>()
      {
        GetSquarePath(100,100,100, Color.Blue),
        GetSquarePath(500,500,200, Color.Red),
        GetLine(300,100),
        GetLine(500,100),
        GetClosedBezierPath(200,400),
      }
    };

    canvas.Layers = new List<CanvasLayer>() { backgroundLayer, gridLayer, objectsLayer };

    SetupToolStripItems(objectsLayer);

    canvas.Layers.ForEach(layer =>
    {
      layerToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(layer.ToString()).Also(it =>
      {
        it.Tag = layer;
        it.Click += (sender, e) => { propertyGrid1.SelectedObject = (sender as ToolStripMenuItem)?.Tag; };
      }));
    });

    propertyGrid1.SelectedObject = canvas.Properties;

    canvas.DrawingMode = new SelectMode(canvas, objectsLayer);

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

  private BezierPath GetClosedBezierPath(int x, int y)
  {
    return new BezierPath().Also(path =>
    {
      path.AddVertex(new Vertex(x, y));
      path.AddVertex(new Vertex(x + 100, y + 200));
      path.AddVertex(new Vertex(x + 300, y + 200));
      path.AddVertex(new Vertex(x + 400, y));
      path.AddVertex(new Vertex(x, y));
      path.FillColor = Color.FromArgb(128, Color.Orange);
    });
  }
  private BezierPath GetLine(int x, int y)
  {
    return new BezierPath().Also(line =>
    {
      line.AddVertex(new Vertex(x, y));
      line.AddVertex(new Vertex(x + 100, y + 200));
      line.AddVertex(new Vertex(x + 200, y));
    });
  }

  private BezierPath GetSquarePath(int x, int y, int size, Color fillColor)
  {
    return new BezierPath().Also(path =>
    {
      path.FillColor = fillColor;
      path.AddVertex(new Vertex(x, y));
      path.AddVertex(new Vertex(x + size, y));
      path.AddVertex(new Vertex(x + size, y + size));
      path.AddVertex(new Vertex(x, y + size));
      path.AddVertex(new Vertex(x, y)); // Close the square
    });
  }

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

  private void AlignToolStripMenuItem_Click(object sender, System.EventArgs e) => canvas.AlignToSnapInterval();

  private void bringToFrontToolStripMenuItem_Click(object sender, EventArgs e) => canvas.BringToFront();

  private void SetupToolStripItems(CanvasLayer canvasLayer)
  {
    ModeGroupManager = new ToolStripItemCheckboxGroupManager(toolStripStatusLabel1)
    {
      OnClick = item =>
      {
        if (item.Tag is DrawingMode mode)
        {
          propertyGrid1.SelectedObject = mode.DefaultObject;
          canvas.DrawingMode.Reset();
          canvas.DrawingMode = mode;
        }
      },
    };
    ModeGroupManager.Create<Select>(new ToolStripItem[] { toolStripButton1, toolStripMenuItem1 }).Also(group => { group.Tag = new SelectMode(canvas, canvasLayer); });
    ModeGroupManager.Create<Line>(new ToolStripItem[] { toolStripButton2, toolStripMenuItem2 }).Also(group => { group.Tag = new LineMode(canvas, canvasLayer, new BezierPath()); });
    ModeGroupManager.Create<Group.Rectangle>(new ToolStripItem[] { toolStripButton3, toolStripMenuItem3 }).Also(group => { group.Tag = new RectangleMode(canvas, canvasLayer, new BezierPath()); });

    GroupManager = new ToolStripItemGroupManager(toolStripStatusLabel1)
    {
      OnClick = item =>
      {
        if (item is Fit) canvas.Fit();
      },
    };
    GroupManager.Create<Fit>(new ToolStripItem[] { fitToolStripButton, fitToolStripMenuItem });
  }
}
