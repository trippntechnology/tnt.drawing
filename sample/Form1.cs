using Microsoft.Win32;
using Sample.Group;
using TNT.Commons;
using TNT.Drawing;
using TNT.Drawing.DrawingModes;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;
using TNT.Drawing.Objects;
using TNT.ToolStripItemManager;
using TNT.Utilities;


namespace Sample;

public partial class Form1 : Form
{
  private ApplicationRegistry? _applicationRegistery = null;
  private readonly Canvas _canvas;

  public Form1()
  {
    InitializeComponent();

    _applicationRegistery = new ApplicationRegistry(this, Registry.CurrentUser, "Tripp'n Technology", "CenteredDrawing");
    _canvas = new Canvas(toolStripContainer1.ContentPanel);
    _canvas.Properties = new CanvasProperties(); ;

    var backgroundLayer = new CanvasLayer(_canvas)
    {
      Name = "Background",
      CanvasObjects = new List<CanvasObject>() { GetSquarePath(150, 150, 50, Color.Green) },
      BackgroundColor = Color.White,
    };

    var gridLayer = new GridLayer(_canvas) { Name = "Grid", LineColor = Color.Aqua };

    var objectsLayer = new CanvasLayer(_canvas)
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

    _canvas.Layers = new List<CanvasLayer>() { backgroundLayer, gridLayer, objectsLayer };

    SetupToolStripItems(objectsLayer);

    _canvas.Layers.ForEach(layer =>
    {
      layerToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(layer.ToString()).Also(it =>
      {
        it.Tag = layer;
        it.Click += (sender, e) => { propertyGrid1.SelectedObject = (sender as ToolStripMenuItem)?.Tag; };
      }));
    });

    propertyGrid1.SelectedObject = _canvas.Properties;

    _canvas.DrawingMode = new SelectMode(_canvas, objectsLayer);

    _canvas.OnSelected = (objs) =>
    {
      try
      {
        propertyGrid1.SelectedObjects = objs.ToArray();
      }
      catch (System.Exception)
      {
      }
    };

    _canvas.OnFeedbackChanged = (feedback) =>
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

  private void LineToolStripMenuItem_Click(object? sender, System.EventArgs e)
  {
    if (sender is ToolStripMenuItem menuItem)
    {
      if (menuItem.Tag is DrawingMode mode)
      {
        propertyGrid1.SelectedObject = mode.DefaultObject;
        _canvas.DrawingMode.Reset();
        _canvas.DrawingMode = mode;
      }
    }
  }

  private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) => _canvas.DrawingMode.Layer.Also(layer => _canvas.Refresh(layer));

  private void AlignToolStripMenuItem_Click(object sender, System.EventArgs e) => _canvas.AlignToSnapInterval();

  private void bringToFrontToolStripMenuItem_Click(object sender, EventArgs e) => _canvas.BringToFront();

  private void SetupToolStripItems(CanvasLayer canvasLayer)
  {
    var modeGroupManager = new ToolStripItemCheckboxGroupManager(toolStripStatusLabel1)
    {
      OnClick = item =>
      {
        if (item.Tag is DrawingMode mode)
        {
          propertyGrid1.SelectedObject = mode.DefaultObject;
          _canvas.DrawingMode.Reset();
          _canvas.DrawingMode = mode;
        }
      },
    };
    modeGroupManager.Create<Select>(new ToolStripItem[] { toolStripButton1, toolStripMenuItem1 }).Also(group => { group.Tag = new SelectMode(_canvas, canvasLayer); });
    modeGroupManager.Create<Line>(new ToolStripItem[] { toolStripButton2, toolStripMenuItem2 }).Also(group => { group.Tag = new LineMode(_canvas, canvasLayer, new BezierPath()); });
    modeGroupManager.Create<Group.Rectangle>(new ToolStripItem[] { toolStripButton3, toolStripMenuItem3 }).Also(group => { group.Tag = new RectangleMode(_canvas, canvasLayer, new BezierPath()); });

    var menuGroupManager = new ToolStripItemGroupManager(toolStripStatusLabel1)
    {
      OnClick = item =>
      {
        if (item is Fit)
        {
          _canvas.Fit();
        }
        else if (item is SaveFile)
        {
          using (var sfd = new System.Windows.Forms.SaveFileDialog())
          {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
              var json = Json.serializeObject(_canvas.State);
              File.WriteAllText(sfd.FileName, json);
            }
          }
        }
        else if (item is OpenFile)
        {
          using (var ofd = new System.Windows.Forms.OpenFileDialog())
          {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
              var json = File.ReadAllText(ofd.FileName);
              Json.deserializeJson<CanvasState>(json)?.Also(state =>
                _canvas.State = state
              );
            }
          }
        }
      },
    };
    menuGroupManager.Create<Fit>(new ToolStripItem[] { fitToolStripButton, fitToolStripMenuItem });
    menuGroupManager.Create<SaveFile>(new ToolStripItem[] { saveToolStripMenuItem1 });
    menuGroupManager.Create<OpenFile>(new ToolStripItem[] { openToolStripMenuItem1 });
  }
}
