using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes
{
  /// <summary>
  /// <see cref="DrawingMode"/> to draw a line
  /// </summary>
  public class LineMode : DrawingMode
  {
    private Line? ActiveLine = null;
    private Vertex? ActiveVertex = null;
    private CanvasPoint? Marker = null;

    /// <summary>
    /// The <see cref="Line"/> that gets created by default
    /// </summary>
    public override CanvasObject DefaultObject { get; } = new Line();

    /// <summary>
    /// Initialization constructor
    /// </summary>
    public LineMode(CanvasLayer layer) : base(layer) { }

    /// <summary>
    /// Clears the state maintained by <see cref="LineMode"/>
    /// </summary>
    public override void Reset()
    {
      ActiveLine = null;
      ActiveVertex = null;
      Marker = null;
      base.Reset();
    }

    /// <summary>
    /// Creates a new line segment
    /// </summary>
    public override void OnMouseClick(MouseEventArgs e, Keys modifierKeys)
    {
      base.OnMouseClick(e, modifierKeys);
      if (Canvas == null) return;

      var location = Canvas.SnapToInterval ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

      if (e.Button == MouseButtons.Left)
      {
        if (ActiveLine == null)
        {
          var line1 = new Line();

          (DefaultObject.Copy() as Line)?.Also(ActiveLine =>
          {
            ActiveLine.IsSelected = true;
            ActiveLine.AddVertex(new Vertex(location));
            ActiveVertex = new Vertex(location);
            ActiveLine.AddVertex(ActiveVertex);
            Refresh();
          });
        }
        else if (ActiveVertex != null)
        {
          ActiveVertex = new Vertex(location);
          ActiveLine.AddVertex(ActiveVertex);
          Refresh();
        }
      }
      else if (e.Button == MouseButtons.Right && ActiveVertex != null && ActiveLine != null)
      {
        ActiveLine.RemoveVertex(ActiveVertex);
        ActiveVertex = ActiveLine.PointsArray.Last() as Vertex;
        if (ActiveLine.PointsArray.Length < 2)
        {
          ActiveLine = null;
          ActiveVertex = null;
        }
        Refresh();
      }
    }

    /// <summary>
    /// Completes the <see cref="Line"/> and adds it to the <see cref="Canvas"/>
    /// </summary>
    public override void OnMouseDoubleClick(MouseEventArgs e)
    {
      base.OnMouseDoubleClick(e);

      if (ActiveLine != null)
      {
        // Remove last vertex
        ActiveVertex?.Also(it => ActiveLine.RemoveVertex(it));
        ActiveLine.IsSelected = false;
        Layer?.CanvasObjects?.Add(ActiveLine);
        ActiveVertex = null;
        ActiveLine = null;
        Refresh(Layer);
      }
    }

    /// <summary>
    /// Moves the <see cref="ActiveVertex"/> or <see cref="Marker"/>
    /// </summary>
    public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
    {
      base.OnMouseMove(e, modifierKeys);
      if (Canvas == null) return;

      var location = Canvas.SnapToInterval ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

      if (ActiveVertex != null)
      {
        ActiveVertex.MoveTo(location, modifierKeys);
        Marker = null;
      }
      else
      {
        if (Marker == null) Marker = new CanvasPoint();
        Marker.MoveTo(location, modifierKeys);
      }
      Invalidate();
    }

    /// <summary>
    ///  Draws the <see cref="Marker"/> or <see cref="ActiveLine"/>
    /// </summary>
    /// <param name="e"></param>
    public override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Marker?.Draw(e.Graphics);
      ActiveLine?.Draw(e.Graphics);
    }
  }
}
