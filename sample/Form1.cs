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
  private readonly DrawingMode _selectMode = new DrawingMode(new ObjectLayer());
  private readonly DrawingMode _rectangularMode = new DrawingMode(new ObjectLayer());
  private readonly DrawingMode _lineMode = new DrawingMode(new ObjectLayer());

  public Form1()
  {
    InitializeComponent();

    _applicationRegistery = new ApplicationRegistry(this, Registry.CurrentUser, "Tripp'n Technology", "CenteredDrawing");

    var backgroundLayer = new CanvasLayer()
    {
      Name = "Background",
      BackgroundColor = Color.White,
    };

    var gridLayer = new GridLayer() { Name = "Grid", LineColor = Color.Aqua };

    var objectsLayer = new ObjectLayer()
    {
      Name = "Object",
      CanvasObjects = new List<CanvasObject>()
      {
        //GetSquarePath(100,100,100, Color.Blue),
        //GetSquarePath(500,500,200, Color.Red),
        //GetLine(300,100),
        //GetLine(500,100),
        GetClosedBezierPath(200,400),
      }
    };

    _lineMode = new LineMode(objectsLayer, new BezierPath());
    _rectangularMode = new RectangleMode(objectsLayer, new BezierPath());
    _selectMode = new SelectMode(objectsLayer);

    _canvas = new Canvas(toolStripContainer1.ContentPanel)
    {
      Layers = new List<CanvasLayer>() { backgroundLayer, gridLayer, objectsLayer },
      Properties = new CanvasProperties(),
      DrawingMode = _selectMode,
      OnSelected = (objs) => { propertyGrid1.SelectedObjects = objs.ToArray(); },
      OnFeedbackChanged = (feedback) =>
      {
        Cursor = feedback.Cursor;
        toolStripStatusLabel1.Text = feedback.Hint;
      },
    };

    SetupToolStripItems();

    _canvas.Layers.ForEach(layer =>
    {
      layerToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(layer.ToString()).Also(it =>
      {
        it.Tag = layer;
        it.Click += (sender, e) => { propertyGrid1.SelectedObject = (sender as ToolStripMenuItem)?.Tag; };
      }));
    });
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
        _canvas.DrawingMode.Reset(_canvas);
        _canvas.DrawingMode = mode;
      }
    }
  }

  private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) => _canvas.DrawingMode.Layer.Also(layer => _canvas.Refresh(layer));

  private void SetupToolStripItems()
  {
    var modeGroupManager = new ToolStripItemCheckboxGroupManager(toolStripStatusLabel1)
    {
      OnClick = item =>
      {
        if (item.Tag is DrawingMode mode)
        {
          propertyGrid1.SelectedObject = mode.DefaultObject;
          _canvas.DrawingMode.Reset(_canvas);
          _canvas.DrawingMode = mode;
        }
      },
    };
    modeGroupManager.Create<Select>(new ToolStripItem[] { toolStripButton1, toolStripMenuItem1 }).Also(group => { group.Tag = _selectMode; });
    modeGroupManager.Create<Line>(new ToolStripItem[] { toolStripButton2, toolStripMenuItem2 }).Also(group => { group.Tag = _lineMode; });
    modeGroupManager.Create<Group.Rectangle>(new ToolStripItem[] { toolStripButton3, toolStripMenuItem3 }).Also(group => { group.Tag = _rectangularMode; });

    var menuGroupManager = new ToolStripItemGroupManager(toolStripStatusLabel1)
    {
      OnClick = item =>
      {
        switch (item)
        {
          case Fit:
            _canvas.Fit();
            break;
          case SaveFile:
            Save();
            break;
          case OpenFile:
            Open();
            break;
          case AlignToGrid:
            _canvas.AlignToSnapInterval();
            break;
          case Group.BringToFront:
            _canvas.BringToFront();
            break;
        }
      },
    };
    menuGroupManager.Create<Fit>(new ToolStripItem[] { fitToolStripButton, fitToolStripMenuItem });
    menuGroupManager.Create<SaveFile>(new ToolStripItem[] { saveToolStripMenuItem, saveToolStripButton });
    menuGroupManager.Create<OpenFile>(new ToolStripItem[] { openToolStripMenuItem, openToolStripButton });
    menuGroupManager.Create<AlignToGrid>(new ToolStripItem[] { alignToolStripButton, alignToolStripMenuItem });
    menuGroupManager.Create<BringToFront>(new ToolStripItem[] { bringToFrontToolStripButton, bringToFrontToolStripMenuItem });
  }

  private void Save()
  {
    using var sfd = new System.Windows.Forms.SaveFileDialog();
    if (sfd.ShowDialog() == DialogResult.OK)
    {
      var json = Json.serializeObject(_canvas.State);
      File.WriteAllText(sfd.FileName, json);
    }
  }

  private void Open()
  {
    using var ofd = new System.Windows.Forms.OpenFileDialog();
    if (ofd.ShowDialog() == DialogResult.OK)
    {
      var json = File.ReadAllText(ofd.FileName);
      Json.deserializeJson<CanvasState>(json)?.Also(state => _canvas.State = state);
    }
  }
}
