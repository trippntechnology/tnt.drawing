﻿using System;
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

namespace TNT.Drawing
{
  /// <summary>
  /// <see cref="Control"/> that provides a grid that can be used to draw on
  /// </summary>
  public class Canvas : Control
  {
    /// <summary>
    /// Delegate that is called to signal that one or more <see cref="object"/> have been
    /// selected within the <see cref="Canvas"/>
    /// </summary>
    public Action<List<object>> OnSelected = (obj) => { };

    /// <summary>
    /// Delegate that is called to signal when feedback has changed within the <see cref="Canvas"/>
    /// </summary>
    public Action<Feedback> OnFeedbackChanged = (feedback) => { };

    private const int MINIMUM_PADDING = 1000;
    private const int PADDING = 50;

    private Rectangle _LayerRect = Rectangle.Empty;
    private bool FitOnPaint = false;
    private bool AdjustPostion = false;
    private KeyEventArgs? keyEventArgs = null;
    private Point PreviousCursorPosition = Point.Empty;
    private Point PreviousGridPosition;
    private ScrollableControl? ScrollableParent = null;
    private CanvasProperties _Properties = new CanvasProperties();
    private List<CanvasLayer> _Layers = new List<CanvasLayer>();

    /// <summary>
    /// Persisted properties
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CanvasProperties Properties
    {
      get { return _Properties; }
      set
      {
        _Properties = value;
        _Properties.OnPropertyChanged += (name, val) => { this.SetProperty(name, val); };
        _Properties.SetProperties(this);
      }
    }

    /// <summary>
    /// <see cref="DrawingMode"/> interacting with the <see cref="Canvas"/>
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DrawingMode DrawingMode { get; set; }

    /// <summary>
    /// <see cref="List{CanvasLayer}"/> managed by the <see cref="Canvas"/>
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<CanvasLayer> Layers { get { return _Layers; } set { _Layers = value; Refresh(); } }

    /// <summary>
    /// The <see cref="ScalePercentage"/> represented as a <see cref="float"/>
    /// </summary>
    [Browsable(false)]
    private float Zoom => ScalePercentage / 100F;

    /// <summary>
    /// Amount the <see cref="Canvas"/> should be scaled
    /// </summary>
    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ScalePercentage { get => Properties.ScalePercentage; set { Properties.ScalePercentage = value; Invalidate(); } }

    /// <summary>
    /// <see cref="CanvasLayer"/> height
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LayerHeight
    {
      get => _LayerRect.Height;
      set
      {
        _LayerRect.Height = value;
        Layers.ForEach(l => l.Height = value);
      }
    }

    /// <summary>
    /// <see cref="CanvasLayer"/> width
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LayerWidth
    {
      get => _LayerRect.Width;
      set
      {
        _LayerRect.Width = value;
        Layers.ForEach(l => l.Width = value);
      }
    }

    /// <summary>
    /// Snap interval
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int SnapInterval { get; set; } = 10;

    /// <summary>
    /// Indicates whether movement should be snapped to <see cref="SnapInterval"/>
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SnapToInterval { get; set; }

    /// <summary>
    /// Scaled grid width
    /// </summary>
    protected int ScaledWidth => (int)(LayerWidth * Zoom);

    /// <summary>
    /// Scaled grid height
    /// </summary>
    protected int ScaledHeight => (int)(LayerHeight * Zoom);

    /// <summary>
    /// Initializes a <see cref="Canvas"/>
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
      ScrollableParent = (Parent as ScrollableControl);
      ScrollableParent?.Also(it => it.AutoScroll = true);

      // DrawingMode can't be null so set an initial DrawingMode
      DrawingMode = new DrawingMode(this, new CanvasLayer(this));
    }

    /// <summary>
    /// Fits the grid within the parent
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
      ScrollableParent?.Also(it => it.AutoScrollPosition = position);
    }

    /// <summary>
    /// Aligns all selected objects to the <see cref="SnapInterval"/>
    /// </summary>
    public void AlignToSnapInterval()
    {
      var selectedObjects = Layers.SelectMany(l => l.GetSelected()).ToList();
      selectedObjects.ForEach(o => o.Align(SnapInterval));
      Invalidate();
    }

    public new void BringToFront()
    {
      Layers.ForEach(layer =>
      {
        var selectedObjects = layer.GetSelected();
        selectedObjects.ForEach(obj =>
        {
          layer.CanvasObjects.Remove(obj);
          layer.CanvasObjects.Add(obj);
        });
      });

      Invalidate();
    }

    /// <summary>
    /// Draws the grid on the canvas
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
      UpdateClientDimensions();
      var graphics = CreateTransformedGraphics(e.Graphics);

      // Draw layers
      Layers.ForEach(l => l.Draw(graphics));

      if (AdjustPostion)
      {
        var previousCanvasPosition = PreviousGridPosition.ToCanvasCoordinates(graphics);
        var currentCanvasPosition = PointToClient(Cursor.Position);
        RepositionToAlignWithMouse(previousCanvasPosition, currentCanvasPosition);
        AdjustPostion = false;
      }

      if (!FitOnPaint)
      {
        FitOnPaint = true;
        Fit();
      }

      DrawingMode.OnDraw(graphics);

      base.OnPaint(e);
    }

    /// <summary>
    /// Repositions the <see cref="Canvas"/> to keep the mouse at the same postion within the <see cref="Canvas"/>
    /// </summary>
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

      ScrollableParent?.Also(it => it.AutoScrollPosition = new Point(-newLocation.X, -newLocation.Y));
    }

    /// <summary>
    /// Updates the client's dimensions
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
    /// Focus this control when mouse movements are detected
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      var graphics = CreateTransformedGraphics();
      var mea = Transform(e, graphics);

      if (keyEventArgs?.KeyCode != Keys.Space) DrawingMode.OnMouseMove(mea, ModifierKeys);

      var currentCursorPosition = Cursor.Position;
      var mousePosition = new Point(e.X, e.Y);
      PreviousGridPosition = mousePosition.ToGridCoordinates(graphics);

      if (keyEventArgs?.KeyCode == Keys.Space)
      {
        RepositionToAlignWithMouse(PreviousCursorPosition, currentCursorPosition);
      }

      PreviousCursorPosition = currentCursorPosition;

      Focus();
      base.OnMouseMove(e);
    }

    /// <summary>
    /// Forces control to immediately redraw itself
    /// </summary>
    public void Refresh(CanvasLayer layer)
    {
      base.Refresh();
    }

    /// <summary>
    /// Sets <see cref="KeyEventArgs"/>
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
      keyEventArgs = e;
      switch (keyEventArgs.KeyCode)
      {
        case Keys.Space:
          Cursor = Cursors.Hand;
          break;
        default:
          DrawingMode.OnKeyDown(e);
          break;
      }
    }

    /// <summary>
    /// Sets <see cref="KeyEventArgs"/>
    /// </summary>
    protected override void OnKeyUp(KeyEventArgs e)
    {
      keyEventArgs = null;
      Cursor = Cursors.Default;
      DrawingMode.OnKeyUp(e);
    }

    /// <summary>
    /// Relays the OnMouseDoubleClick to the <see cref="DrawingMode"/>
    /// </summary>
    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
      base.OnMouseDoubleClick(e);
      DrawingMode.OnMouseDoubleClick(e);
    }

    /// <summary>
    /// Relays the OnMouseDown to the <see cref="DrawingMode"/>
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e)
    {
      MouseEventArgs mea = Transform(e);
      DrawingMode.OnMouseDown(mea, ModifierKeys);
      base.OnMouseDown(e);
    }

    /// <summary>
    /// Relays the OnMouseUp to the <see cref="DrawingMode"/>
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      var graphics = CreateTransformedGraphics();
      var mea = Transform(e, graphics);
      DrawingMode.OnMouseUp(mea, ModifierKeys);
    }

    /// <summary>
    /// Transforms the location within the <see cref="MouseEventArgs"/> using the <paramref name="graphics"/>
    /// </summary>
    /// <returns><see cref="MouseEventArgs"/> with the transformed location</returns>
    private MouseEventArgs Transform(MouseEventArgs e, Graphics? graphics = null)
    {
      graphics = graphics ?? CreateTransformedGraphics();
      var layerPoint = e.Location.ToGridCoordinates(graphics); //.Snap(10);
      return new MouseEventArgs(e.Button, e.Clicks, layerPoint.X, layerPoint.Y, e.Delta);
    }

    /// <summary>
    /// Changes <see cref="ScalePercentage"/>
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseWheel(MouseEventArgs e)
    {
      // Size of a scroll delta (seems to be 120)
      var wheelDelta = SystemInformation.MouseWheelScrollDelta;
      // Change, smaller (-1) or larger (1)
      var change = e.Delta / wheelDelta;
      // Amount of change that should be applied to the scale percentage
      //var detents = change / (keyEventArgs?.Shift == true ? 100.0f : 10.0f);
      var detents = change * (keyEventArgs?.Shift == true ? 1 : 10);

      if (keyEventArgs?.Control == true && ScalePercentage + detents > 0)
      {
        // Adjust position when Paint() is called
        AdjustPostion = true;
        var graphics = CreateTransformedGraphics();
        var positionOnCanvas = new Point(e.X, e.Y);
        PreviousGridPosition = positionOnCanvas.ToGridCoordinates(graphics);
        ScalePercentage += detents;
        (e as HandledMouseEventArgs)?.Let(h => h.Handled = true);
      }
    }

    /// <summary>
    /// Redraws <see cref="Canvas"/> when the parent's size changes.
    /// </summary>
    private void OnParentResize(object? sender, EventArgs e) => Refresh();

    /// <summary>
    /// Called when objects have been selected
    /// </summary>
    public void OnObjectsSelected(List<object> objs)
    {
      var selected = objs.Count == 0 ? new List<Object>() { Properties } : objs;
      OnSelected(selected);
    }

    /// <summary>
    /// Returns a <see cref="Graphics"/> that has been transformed
    /// </summary>
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
}