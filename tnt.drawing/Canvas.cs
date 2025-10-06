using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.DrawingModes;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;

namespace TNT.Drawing;

/// <summary>
/// Represents the main grid-based drawing surface for TNT.Drawing, supporting multiple layers, drawing modes, coordinate transformations, and interactive user input.
/// Handles mouse and keyboard events, manages zoom and pan, and provides event hooks for selection and feedback.
/// </summary>
public class Canvas : Control
{
  // Consts
  private const int MINIMUM_PADDING = 1000;
  private const int PADDING = 50;

  // Fields
  private Rectangle _layerRect = Rectangle.Empty;
  private bool _fitOnPaint = false;
  private bool _adjustPostion = false;
  private KeyEventArgs? _keyEventArgs = null;
  private Point _previousCursorPosition = Point.Empty;
  private Point _previousGridPosition;
  private ScrollableControl? _scrollableParent = null;
  private CanvasProperties? _properties;
  private List<CanvasLayer>? _layers;

  // Events/Delegates

  /// <summary>
  /// Invoked when one or more objects are selected within the canvas.
  /// </summary>
  public required Action<List<object>> OnSelected = (obj) => { };

  /// <summary>
  /// Invoked when feedback (cursor/hint) changes within the canvas.
  /// </summary>
  public required Action<Feedback> OnFeedbackChanged = (feedback) => { };

  // Constructors
  /// <summary>
  /// Initializes a new instance of the <see cref="Canvas"/> class and attaches it to the specified parent control.
  /// </summary>
  public Canvas(Control parent)
      : base()
  {
    CanvasPanel canvasPanel = new CanvasPanel(parent);
    Parent = canvasPanel;
    Width = parent.Width;
    Height = parent.Height;

    DoubleBuffered = true;
    parent.SizeChanged += OnParentResize;
    _scrollableParent = (Parent as ScrollableControl);
    _scrollableParent?.Also(it => it.AutoScroll = true);

    // DrawingMode can't be null so set an initial DrawingMode
    DrawingMode = new DrawingMode(new ObjectLayer());
  }

  // Properties

  /// <summary>
  /// Gets or sets the persisted properties for the canvas.
  /// </summary>
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public required CanvasProperties Properties
  {
    get { return _properties ?? throw new NullReferenceException(); }
    set
    {
      _properties = value;
      _properties.OnPropertyChanged += (name, val) => { this.SetProperty(name, val); };
      _properties.SetProperties(this);
    }
  }

  /// <summary>
  /// Gets or sets the current drawing mode interacting with the canvas.
  /// </summary>
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public required DrawingMode DrawingMode { get; set; }

  /// <summary>
  /// Gets or sets the list of layers managed by the canvas.
  /// </summary>
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public required List<CanvasLayer> Layers
  {
    get { return _layers ?? throw new NullReferenceException(); }
    set { _layers = value; Refresh(); }
  }

  /// <summary>
  /// Gets the scale factor as a float (percentage divided by 100).
  /// </summary>
  [Browsable(false)]
  private float Zoom => ScalePercentage / 100F;

  /// <summary>
  /// Gets or sets the scale percentage for zooming the canvas.
  /// </summary>
  [Category("Appearance")]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int ScalePercentage { get => Properties.ScalePercentage; set { Properties.ScalePercentage = value; Invalidate(); } }

  /// <summary>
  /// Gets or sets the height of the canvas layer.
  /// </summary>
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int LayerHeight
  {
    get => _layerRect.Height;
    set
    {
      _layerRect.Height = value;
      Layers.ForEach(l => l.Height = value);
    }
  }

  /// <summary>
  /// Gets or sets the width of the canvas layer.
  /// </summary>
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int LayerWidth
  {
    get => _layerRect.Width;
    set
    {
      _layerRect.Width = value;
      Layers.ForEach(l => l.Width = value);
    }
  }

  /// <summary>
  /// Gets or sets the snap interval for object alignment.
  /// </summary>
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int SnapInterval { get; set; } = 10;

  /// <summary>
  /// Gets or sets whether movement should be snapped to the snap interval.
  /// </summary>
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public bool SnapToInterval { get; set; }

  /// <summary>
  /// Gets the scaled grid width based on the zoom factor.
  /// </summary>
  protected int ScaledWidth => (int)(LayerWidth * Zoom);

  /// <summary>
  /// Gets the scaled grid height based on the zoom factor.
  /// </summary>
  protected int ScaledHeight => (int)(LayerHeight * Zoom);

  /// <summary>
  /// Gets or sets the current state of the canvas, including all persisted properties.
  /// </summary>
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public CanvasState State
  {
    get => new CanvasState(Properties, Layers);
    set
    {
      Properties = value.properties;
      value.layers.ForEach(layer =>
      {
        var thisLayer = Layers.Find(l => l.Name == layer.Name);
        thisLayer?.CopyFrom(layer);
      });
    }
  }

  // Public Methods
  /// <summary>
  /// Fits the grid within the parent control, adjusting scale and scroll position.
  /// </summary>
  public void Fit()
  {
    var parent = Parent!;
    var parentWidth = parent.Width;
    var parentHeight = parent.Height;
    var gridRatio = LayerWidth / (LayerHeight * 1F);
    var parentRatio = parentWidth / (parentHeight * 1F);
    float newScale;

    if (gridRatio > parentRatio)
    {
      // Width is greater
      newScale = 100 * (parentWidth * 1F) / (LayerWidth + PADDING * 2);
    }
    else
    {
      // Height is greater
      newScale = 100 * (parentHeight * 1F) / (LayerHeight + PADDING * 2);
    }

    ScalePercentage = Convert.ToInt32(newScale);
    var position = new Point(-(parent.Width / 2 - Width / 2), -(parent.Height / 2 - Height / 2));
    _scrollableParent?.Also(it => it.AutoScrollPosition = position);
  }

  /// <summary>
  /// Aligns all selected objects in all object layers to the current canvas snap interval.
  /// Triggers a redraw of the canvas after alignment.
  /// </summary>
  public void AlignToSnapInterval()
  {
    Layers
      .OfType<ObjectLayer>()
      .ToList()
      .ForEach(it => it.AlignToSnapInterval(SnapInterval));
    Invalidate();
  }

  /// <summary>
  /// Moves all selected objects in all object layers to the front of the z-order in their respective layers.
  /// Triggers a redraw of the canvas after alignment.
  /// </summary>
  public new void BringToFront()
  {
    Layers
      .OfType<ObjectLayer>()
      .ToList()
      .ForEach(layer => layer.BringToFront());
    Invalidate();
  }

  /// <summary>
  /// Handles object selection and invokes the selection event.
  /// </summary>
  /// <param name="objs">The list of selected objects.</param>
  public void OnObjectsSelected(List<object> objs)
  {
    var selected = objs.Count == 0 ? new List<Object>() { Properties } : objs;
    OnSelected(selected);
  }

  // Protected/Override Methods
  /// <summary>
  /// Paints the canvas, drawing all layers and the current drawing mode.
  /// </summary>
  /// <param name="e">The paint event arguments.</param>
  protected override void OnPaint(PaintEventArgs e)
  {
    UpdateClientDimensions();
    var graphics = CreateTransformedGraphics(e.Graphics);

    // Draw layers
    Layers.ForEach(l => l.Draw(graphics, SnapInterval));

    if (_adjustPostion)
    {
      var previousCanvasPosition = _previousGridPosition.ToCanvasCoordinates(graphics);
      var currentCanvasPosition = PointToClient(Cursor.Position);
      RepositionToAlignWithMouse(previousCanvasPosition, currentCanvasPosition);
      _adjustPostion = false;
    }

    if (!_fitOnPaint)
    {
      _fitOnPaint = true;
      Fit();
    }

    DrawingMode.OnDraw(graphics, this);

    base.OnPaint(e);
  }

  /// <summary>
  /// Focuses this control when mouse movements are detected and handles mouse move events.
  /// </summary>
  /// <param name="e">The mouse event arguments.</param>
  protected override void OnMouseMove(MouseEventArgs e)
  {
    using var graphics = CreateTransformedGraphics();
    var transformedMEA = Transform(e, graphics);

    if (_keyEventArgs?.KeyCode != Keys.Space) DrawingMode.OnMouseMove(transformedMEA, ModifierKeys, this);

    var currentCursorPosition = Cursor.Position;
    _previousGridPosition = transformedMEA.Location;

    if (_keyEventArgs?.KeyCode == Keys.Space)
    {
      RepositionToAlignWithMouse(_previousCursorPosition, currentCursorPosition);
    }

    _previousCursorPosition = currentCursorPosition;

    Focus();
    base.OnMouseMove(e);
  }

  /// <summary>
  /// Handles key down events, switching to pan mode if space is pressed.
  /// </summary>
  /// <param name="e">The key event arguments.</param>
  protected override void OnKeyDown(KeyEventArgs e)
  {
    _keyEventArgs = e;
    switch (e.KeyCode)
    {
      case Keys.Space:
        Cursor = Cursors.Hand;
        break;
      default:
        DrawingMode.OnKeyDown(_previousGridPosition, ModifierKeys, this);
        break;
    }
  }

  /// <summary>
  /// Handles key up events, restoring cursor and passing to drawing mode.
  /// </summary>
  /// <param name="e">The key event arguments.</param>
  protected override void OnKeyUp(KeyEventArgs e)
  {
    _keyEventArgs = null;
    //Cursor = Cursors.Default;
    DrawingMode.OnKeyUp(_previousGridPosition, ModifierKeys, this);
  }

  /// <summary>
  /// Relays the mouse double-click event to the current drawing mode.
  /// </summary>
  /// <param name="e">The mouse event arguments.</param>
  protected override void OnMouseDoubleClick(MouseEventArgs e)
  {
    base.OnMouseDoubleClick(e);
    DrawingMode.OnMouseDoubleClick(e);
  }

  /// <summary>
  /// Relays the mouse down event to the current drawing mode.
  /// </summary>
  /// <param name="e">The mouse event arguments.</param>
  protected override void OnMouseDown(MouseEventArgs e)
  {
    MouseEventArgs mea = Transform(e);
    DrawingMode.OnMouseDown(mea, ModifierKeys, this);
    base.OnMouseDown(e);
  }

  /// <summary>
  /// Relays the mouse up event to the current drawing mode.
  /// </summary>
  /// <param name="e">The mouse event arguments.</param>
  protected override void OnMouseUp(MouseEventArgs e)
  {
    base.OnMouseUp(e);
    using var graphics = CreateTransformedGraphics();
    var mea = Transform(e, graphics);
    DrawingMode.OnMouseUp(mea, ModifierKeys, this);
  }

  /// <summary>
  /// Handles mouse wheel events for zooming the canvas.
  /// </summary>
  /// <param name="e">The mouse event arguments.</param>
  protected override void OnMouseWheel(MouseEventArgs e)
  {
    // Size of a scroll delta (seems to be 120)
    var wheelDelta = SystemInformation.MouseWheelScrollDelta;
    // Change, smaller (-1) or larger (1)
    var change = e.Delta / wheelDelta;
    // Amount of change that should be applied to the scale percentage
    //var detents = change / (keyEventArgs?.Shift == true ? 100.0f : 10.0f);
    var detents = change * (_keyEventArgs?.Shift == true ? 1 : 10);

    if (_keyEventArgs?.Control == true && ScalePercentage + detents > 0)
    {
      // Adjust position when Paint() is called
      _adjustPostion = true;
      using var graphics = CreateTransformedGraphics();
      var positionOnCanvas = new Point(e.X, e.Y);
      _previousGridPosition = positionOnCanvas.ToGridCoordinates(graphics);
      ScalePercentage += detents;
      (e as HandledMouseEventArgs)?.Let(h => h.Handled = true);
    }
  }

  // Private Methods
  /// <summary>
  /// Repositions the canvas to keep the mouse at the same position within the canvas after zoom/pan.
  /// </summary>
  /// <param name="previousPosition">The previous mouse position in canvas coordinates.</param>
  /// <param name="currentPosition">The current mouse position in canvas coordinates.</param>
  private void RepositionToAlignWithMouse(Point previousPosition, Point currentPosition)
  {
    var parent = Parent!;
    var min = 0;
    var deltaPosition = previousPosition.Subtract(currentPosition);
    var dx = -deltaPosition.X;
    var dy = -deltaPosition.Y;
    var newLocation = Location.Subtract(deltaPosition);

    var newLeft = Left + dx;
    var newRight = Right + dx;
    var newTop = Top + dy;
    var newBottom = Bottom + dy;

    if (newLeft > min)
    {
      newLocation.X = min;
    }
    else if (newRight < parent.Width - min)
    {
      newLocation.X = parent.Width - min - Width;
    }

    if (newTop > min)
    {
      newLocation.Y = min;
    }
    else if (newBottom < parent.Height - min)
    {
      newLocation.Y = parent.Height - min - Height;
    }

    _scrollableParent?.Also(it => it.AutoScrollPosition = new Point(-newLocation.X, -newLocation.Y));
  }

  /// <summary>
  /// Updates the client dimensions to fit the drawing canvas and padding.
  /// </summary>
  private void UpdateClientDimensions()
  {
    var parent = Parent!;

    // Adjust the width and height of the canvas to fit the drawing canvas
    var newWidth = ScaledWidth > parent.ClientRectangle.Width ? ScaledWidth : parent.ClientRectangle.Width;
    var newHeight = ScaledHeight > parent.ClientRectangle.Height ? ScaledHeight : parent.ClientRectangle.Height;

    Width = newWidth + MINIMUM_PADDING;
    Height = newHeight + MINIMUM_PADDING;
  }

  /// <summary>
  /// Redraws the canvas when the parent's size changes.
  /// </summary>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnParentResize(object? sender, EventArgs e) => Refresh();

  /// <summary>
  /// Transforms the location within the <see cref="MouseEventArgs"/> using the specified graphics transformation.
  /// </summary>
  /// <param name="e">The mouse event arguments.</param>
  /// <param name="graphics">The graphics context for transformation.</param>
  /// <returns>A <see cref="MouseEventArgs"/> with the transformed location.</returns>
  private MouseEventArgs Transform(MouseEventArgs e, Graphics? graphics = null)
  {
    var disposeGraphics = graphics == null;
    graphics = graphics ?? CreateTransformedGraphics();
    var layerPoint = e.Location.ToGridCoordinates(graphics);
    if (disposeGraphics) { graphics.Dispose(); }
    return new MouseEventArgs(e.Button, e.Clicks, layerPoint.X, layerPoint.Y, e.Delta);
  }

  /// <summary>
  /// Creates and returns a <see cref="Graphics"/> object with the current canvas scale and translation applied.
  /// <para>
  /// This method configures the graphics context so that drawing operations and coordinate conversions
  /// are mapped between grid (world) coordinates and canvas (screen) coordinates using the current zoom and centering.
  /// </para>
  /// <para>
  /// Always use the returned graphics context for rendering layers, objects, and transforming mouse events
  /// to ensure correct coordinate mapping and drawing mode behavior.
  /// </para>
  /// <param name="graphics">Optional. An existing graphics context to apply transformations to. If <c>null</c>, a new graphics context is created for the canvas.</param>
  /// <returns>A <see cref="Graphics"/> object with scale and translation transforms applied for grid-to-canvas mapping. This must be disposed of by the caller.</returns>
  /// </summary>
  /// <param name="graphics">An optional graphics context to apply transformations to.</param>
  /// <returns>The transformed <see cref="Graphics"/> object.</returns>
  private Graphics CreateTransformedGraphics(Graphics? graphics = null)
  {
    graphics = graphics ?? CreateGraphics();
    graphics.SmoothingMode = SmoothingMode.AntiAlias;

    // X translation of the drawing canvas
    var xTranslation = Math.Max((Width - ScaledWidth) / 2, 0);
    // Y translation of the drawing canvas
    var yTranslation = Math.Max((Height - ScaledHeight) / 2, 0);

    // Scale
    graphics.ScaleTransform(Zoom, Zoom, MatrixOrder.Append);
    // Transform
    graphics.TranslateTransform(xTranslation, yTranslation, MatrixOrder.Append);

    return graphics;
  }
}