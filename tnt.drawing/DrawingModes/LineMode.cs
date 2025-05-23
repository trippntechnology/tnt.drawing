using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// <see cref="DrawingMode"/> to draw a line
/// </summary>
public class LineMode(Canvas canvas, CanvasLayer layer) : DrawingMode(canvas, layer)
{
  private Line? ActiveLine = null;
  private Vertex? ActiveVertex = null;
  private CanvasPoint? Marker = null;

  /// <summary>
  /// The <see cref="Line"/> that gets created by default
  /// </summary>
  public override CanvasObject DefaultObject { get; } = new Line();

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
  /// Handles mouse button release events to update the current line drawing state.
  /// <para>
  /// - On left mouse button release: 
  ///   - If no active line exists, creates a new <see cref="Line"/> and adds the initial vertices at the mouse location.
  ///   - If a line is active, adds a new vertex at the released location.
  /// - On right mouse button release: 
  ///   - If a line and vertex are active, removes the last vertex from the line.
  ///   - If the line has fewer than two points after removal, clears the active line and vertex.
  /// </para>
  /// Refreshes the <see cref="Canvas"/> after each operation.
  /// </summary>
  /// <param name="e">Mouse event arguments containing the release location and button information.</param>
  /// <param name="modifierKeys">Modifier keys pressed during the event.</param>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
  {
    // Call the base implementation first
    base.OnMouseUp(e, modifierKeys);

    var location = Canvas.SnapToInterval ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    if (e.Button == MouseButtons.Left)
    {
      if (ActiveLine == null)
      {
        ActiveLine = DefaultObject.Copy() as Line;
        ActiveLine?.Also(line =>
            {
              // Mark the new line as selected and add initial vertices
              line.IsSelected = true;
              line.AddVertex(new Vertex(location));
              ActiveVertex = new Vertex(location);
              line.AddVertex(ActiveVertex);
              Canvas.Refresh();
            }
        );
      }
      // Subsequent clicks only if we have an active vertex
      else if (ActiveVertex != null)
      {
        // Create a new vertex at the clicked location and add it to the line
        ActiveVertex = new Vertex(location);
        ActiveLine.AddVertex(ActiveVertex);
        Canvas.Refresh();
      }
    }
    // Right-click: remove vertices (only when a line and vertex are active)
    else if (e.Button == MouseButtons.Right && ActiveVertex != null && ActiveLine != null)
    {
      // Remove the current active vertex and update to the last point
      ActiveLine.RemoveVertex(ActiveVertex);
      ActiveVertex = ActiveLine.PointsArray.Last() as Vertex;

      // Clean up if the line becomes invalid (fewer than 2 points)
      if (ActiveLine.PointsArray.Length < 2)
      {
        ActiveLine = null;
        ActiveVertex = null;
      }
      Canvas.Refresh();
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
      Canvas.Refresh();
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
    Canvas.Refresh();
  }

  /// <summary>
  /// Draws the <see cref="Marker"/> or <see cref="ActiveLine"/> on the canvas.
  /// </summary>
  /// <param name="graphics">Graphics object used for drawing.</param>
  public override void OnDraw(Graphics graphics)
  {
    base.OnDraw(graphics);
    Marker?.Draw(graphics);
    ActiveLine?.Draw(graphics);
  }
}
