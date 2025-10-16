using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes;

public class LineMode(ObjectLayer layer, BezierPath defaultObject) : DrawingMode(layer, defaultObject)
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

  /// <summary>
  /// Handles mouse button release events for the Line drawing mode.
  /// Adds a new vertex to the current line, closes the path if appropriate, and creates a new BezierPath object.
  /// - Left mouse button: Adds a vertex if not near an existing one, or closes the path if near the first vertex and enough vertices exist.
  /// - CTRL key: Finishes the line if held and at least two vertices exist.
  /// - Right mouse button: Removes the last vertex.
  /// Triggers a canvas redraw and calls the base implementation.
  /// </summary>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys, Canvas canvas)
  {
    if (e.Button == MouseButtons.Left)
    {
      var isClosed = false;
      var newVertex = new Vertex(_activeVertex);

      // Add the new vertex if not near any existing vertex
      if (!_vertices.Any(v => v.IsNear(newVertex)))
      {
        _vertices.Add(newVertex);
      }
      // If enough vertices and new vertex is near the first, close the path
      else if (_vertices.Count > 2 && newVertex.IsNear(_vertices.First()))
      {
        isClosed = true;
        _vertices.Add(newVertex);
      }

      // Finish the line if closed or CTRL is held and at least two vertices exist
      if (isClosed || ((modifierKeys & Keys.Control) == Keys.Control && _vertices.Count > 1))
      {
        // Create a new BezierPath and add each vertex
        var path = (DefaultBezierPath.Clone() as BezierPath)!;
        foreach (var v in _vertices)
        {
          //path.AddVertex(new Vertex(v));
          path.AddVertex(v);
        }
        Layer.CanvasObjects.Add(path);

        _vertices.Clear();
      }
    }
    // Remove the last vertex on right mouse button
    else if (e.Button == MouseButtons.Right)
    {
      _vertices.RemoveAt(_vertices.Count - 1);
    }

    canvas.Invalidate();

    base.OnMouseUp(e, modifierKeys, canvas);
  }

  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys, Canvas canvas)
  {
    base.OnMouseMove(e, modifierKeys, canvas);

    var location = e.Location;

    if (_vertices.Count == 0)
    {
      canvas.OnFeedbackChanged(Feedback.LINE_MODE_INITIAL_VERTEX);
    }
    else if (_vertices.Count > 0)
    {
      // If SHIFT is pressed, constrain to every 15 degrees from the origin vertex
      if ((modifierKeys & Keys.Shift) == Keys.Shift)
      {
        var origin = _vertices.Last().ToPoint;
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

      if ((modifierKeys & Keys.Control) == Keys.Control)
      {
        canvas.OnFeedbackChanged(Feedback.LINE_MODE_SECOND_VERTEX_CTRL);
      }
      else if (_vertices.Count > 0)
      {
        if (_vertices.First().IsMouseOver(location, modifierKeys))
        {
          canvas.OnFeedbackChanged(Feedback.LINE_MODE_CLOSED);
        }
        else
        {
          canvas.OnFeedbackChanged(Feedback.LINE_MODE_SECOND_VERTEX);
        }
      }
    }

    location = canvas.SnapToInterval ? location.Snap(canvas.SnapInterval) : location;

    _activeVertex.MoveTo(location.X, location.Y, modifierKeys, Point.Empty);
    canvas.Invalidate();
  }

  public override void OnDraw(Graphics graphics, Canvas canvas)
  {
    base.OnDraw(graphics, canvas);

    // Draw the lines connecting the vertices
    var points = _vertices.ConvertAll(v => v.ToPoint);
    // Add the active vertex to the end of the points list for drawing
    points.Add(_activeVertex.ToPoint);
    if (points.Count > 1)
    {
      graphics.DrawLines(LinesPen, points.ToArray());
    }
    // Draw the vertices
    _vertices.ForEach(v => v.Draw(graphics));
    // Draw the active vertex
    _activeVertex.Draw(graphics);
  }
}
