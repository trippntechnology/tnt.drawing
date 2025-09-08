using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
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
    var isClosed = false;

    // Add a new vertex at the mouse location
    vertices.Add(new Vertex(activeVertex));

    vertices.Select(v => v as CanvasPoint).ToList().Also(canvasPoints =>
    {
      canvasPoints.FirstOrDefault()?.Also(first => canvasPoints.FindCoincident(first)?.Also(_ => isClosed = true));
    });

    // If CTRL is held, finish the line
    if (isClosed || (modifierKeys & Keys.Control) == Keys.Control && vertices.Count > 1)
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

    var location = e.Location;

    // If SHIFT is pressed, constrain to every 15 degrees from the origin vertex
    if ((modifierKeys & Keys.Shift) == Keys.Shift && vertices.Count > 0)
    {
      var origin = vertices.Last().ToPoint;
      var dx = location.X - origin.X;
      var dy = location.Y - origin.Y;
      var distance = Math.Sqrt(dx * dx + dy * dy);
      if (distance > 0)
      {
        var angle = Math.Atan2(dy, dx); // radians
        var angleDeg = angle * 180.0 / Math.PI;
        var snappedAngleDeg = Math.Round(angleDeg / 15.0) * 15.0;
        var snappedAngleRad = snappedAngleDeg * Math.PI / 180.0;
        var snappedX = origin.X + (int)Math.Round(distance * Math.Cos(snappedAngleRad));
        var snappedY = origin.Y + (int)Math.Round(distance * Math.Sin(snappedAngleRad));
        location = new Point(snappedX, snappedY);
      }
    }

    location = Canvas.SnapToInterval ? location.Snap(Canvas.SnapInterval) : location;

    activeVertex.MoveTo(location.X, location.Y, modifierKeys);
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
