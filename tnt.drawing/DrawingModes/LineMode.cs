using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes;

public class LineMode(Canvas canvas, CanvasLayer layer, BezierPath defaultObject) : DrawingMode(canvas, layer, defaultObject)
{
  // List of vertices for the current line being drawn
  private readonly List<Vertex> currentVertices = new();
  private Vertex activeVertex = new Vertex();

  private BezierPath DefaultBezierPath => (DefaultObject as BezierPath)!;

  private Pen _linesPen = new Pen(Color.Black, 1);
  private Pen LinesPen
  {
    get
    {
      _linesPen.Color = DefaultBezierPath.Color;
      _linesPen.Width = DefaultBezierPath.Width;
      _linesPen.DashStyle = DefaultBezierPath.Style;
      return _linesPen;
    }
  }

  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
  {
    var location = Canvas.SnapToInterval ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    // Add a new vertex at the mouse location
    var vertex = new Vertex(location);
    currentVertices.Add(vertex);

    // If CTRL is held, finish the line
    if ((modifierKeys & Keys.Control) == Keys.Control && currentVertices.Count > 1)
    {
      CommitLine();
    }

    base.OnMouseUp(e, modifierKeys);
  }

  /// <summary>
  /// Commits the current line to the layer and resets the state
  /// </summary>
  private void CommitLine()
  {
    if (currentVertices.Count > 1)
    {
      // Create a new BezierPath and add each vertex
      var path = new BezierPath();
      foreach (var v in currentVertices)
      {
        path.AddVertex(new Vertex(v));
      }
      Layer.CanvasObjects.Add(path);
    }
    currentVertices.Clear();
  }

  public override void OnDraw(Graphics graphics)
  {
    base.OnDraw(graphics);
    var points = currentVertices.ConvertAll(v => v.ToPoint);
    points.Add(activeVertex.ToPoint); // Add the active vertex point
    if (points?.Count > 1){
      graphics.DrawLines(LinesPen, points.ToArray());
    }
    currentVertices.ForEach(v => v.Draw(graphics));
    activeVertex.Draw(graphics);
  }
  public override void Reset()
  {
    currentVertices.Clear();
    base.Reset();
  }

  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    base.OnMouseMove(e, modifierKeys);

    var location = Canvas.SnapToInterval ? e.Location.Snap(Canvas.SnapInterval) : e.Location;
    activeVertex.MoveTo(location, modifierKeys);    Canvas.Invalidate();
  }

  protected override void Log(string msg = "", [CallerMemberName] string callingMethod = "")
  {
    base.Log(msg, callingMethod);
  }
}
