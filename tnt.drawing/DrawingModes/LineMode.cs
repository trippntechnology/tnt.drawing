using System.Drawing;
using System.Windows.Forms;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes;

public class LineMode(Canvas canvas, CanvasLayer layer, BezierPath defaultObject) : DrawingMode(canvas, layer, defaultObject)
{
  private BezierPath DefaultBezierPath => (DefaultObject as BezierPath)!;

  private Pen _linesPen = new Pen(Color.Black, 1);
  private Pen LinesPen
  {
    get
    {
      _linesPen.Color = DefaultBezierPath.LineColor;
      _linesPen.Width = DefaultBezierPath.Width;
      _linesPen.DashStyle = DefaultBezierPath.LineStyle;
      return _linesPen;
    }
  }

  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
  {
    var location = Canvas.SnapToInterval ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    // Add a new vertex at the mouse location
    vertices.Add(new Vertex(location));

    // If CTRL is held, finish the line
    if (modifierKeys == Keys.Control && vertices.Count > 1)
    {
      if (vertices.Count > 1)
      {
        // Create a new BezierPath and add each vertex
        var path = (DefaultBezierPath.Clone() as BezierPath)!;
        foreach (var v in vertices)
        {
          path.AddVertex(new Vertex(v));
        }
        Layer.CanvasObjects.Add(path);
      }
      vertices.Clear();
    }

    Canvas.Invalidate();

    base.OnMouseUp(e, modifierKeys);

  }

  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    base.OnMouseMove(e, modifierKeys);

    var location = Canvas.SnapToInterval ? e.Location.Snap(Canvas.SnapInterval) : e.Location;
    activeVertex.MoveTo(location, modifierKeys);
    Canvas.Invalidate();
  }

  public override void OnDraw(Graphics graphics)
  {
    base.OnDraw(graphics);

    // Draw the lines connecting the vertices
    var points = vertices.ConvertAll(v => v.ToPoint);
    // Add the active vertex to the end of the points list for drawing
    points.Add(activeVertex.ToPoint);
    if (points.Count > 1)
    {
      graphics.DrawLines(LinesPen, points.ToArray());
    }
    // Draw the vertices
    vertices.ForEach(v => v.Draw(graphics));
    // Draw the active vertex
    activeVertex.Draw(graphics);
  }
}
