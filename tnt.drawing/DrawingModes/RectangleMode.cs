using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TNT.Drawing.Extensions;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Provides a drawing mode for creating rectangles (squares) on the canvas.
/// Users can click and drag to define the rectangle's bounds, which is rendered as a closed <see cref="BezierPath"/>.
/// </summary>
public class RectangleMode(CanvasLayer layer, CanvasObject? defaultObject = null) : DrawingMode(layer, defaultObject)
{
  /// <summary>
  /// Gets the default <see cref="BezierPath"/> template used for new rectangles.
  /// </summary>
  private BezierPath DefaultBezierPath => (DefaultObject as BezierPath)!;

  private Pen _linesPen = new Pen(Color.Black, 1);

  /// <summary>
  /// Gets a <see cref="Pen"/> configured with the current line color, width, and style for drawing rectangle outlines.
  /// </summary>
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
  /// Handles the mouse button release event to finalize or start a rectangle.
  /// If vertices exist, creates a closed <see cref="BezierPath"/> and adds it to the layer; otherwise, initializes rectangle vertices.
  /// </summary>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys, Canvas canvas)
  {
    var location = canvas.SnapToInterval ? e.Location.Snap(canvas.SnapInterval) : e.Location;

    if (vertices.Count > 0)
    {
      // Create a new BezierPath and add each vertex
      var path = (DefaultBezierPath.Clone() as BezierPath)!;

      vertices.ForEach(v => path.AddVertex(new Vertex(v)));
      // Close the path by adding the first vertex again
      path.AddVertex(new Vertex(vertices.First()));

      Layer.CanvasObjects.Add(path);

      vertices.Clear();
    }
    else
    {
      vertices.Add(new Vertex(location));
      vertices.Add(new Vertex(location));
      vertices.Add(new Vertex(location));
      vertices.Add(new Vertex(location));
    }

    canvas.Invalidate();

    base.OnMouseDown(e, modifierKeys, canvas);
  }

  /// <summary>
  /// Handles mouse movement to update the preview rectangle as the user drags.
  /// Adjusts the positions of rectangle vertices to reflect the current mouse location.
  /// </summary>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys, Canvas canvas)
  {
    base.OnMouseMove(e, modifierKeys, canvas);

    var location = canvas.SnapToInterval ? e.Location.Snap(canvas.SnapInterval) : e.Location;

    if (vertices.Count != 0)
    {
      var startVertex = vertices.First();

      // Update preview rectangle dimensions
      int width = location.X - vertices.First().X;
      int height = location.Y - vertices.First().Y;

      var p2 = new Point(startVertex.X + width, startVertex.Y);
      var p4 = new Point(startVertex.X, startVertex.Y + height);
      vertices[1].MoveTo(p2.X, p2.Y, Keys.None, Point.Empty);
      vertices[2].MoveTo(location.X, location.Y, Keys.None, Point.Empty);
      vertices[3].MoveTo(p4.X, p4.Y, Keys.None, Point.Empty);
    }
    else
    {
      activeVertex.MoveTo(location.X, location.Y, modifierKeys, Point.Empty);
    }

    canvas.Invalidate();
  }

  /// <summary>
  /// Draws the preview or finalized rectangle on the canvas.
  /// Renders the rectangle fill, outline, and optionally the vertices for visual feedback.
  /// </summary>
  public override void OnDraw(Graphics graphics, Canvas canvas)
  {
    base.OnDraw(graphics, canvas);

    if (vertices.Count < 4)
    {
      activeVertex.Draw(graphics);
      return; // Not enough vertices to draw a rectangle
    }

    // Draw the rectangle using the vertices
    var points = vertices.Select(v => v.ToPoint).ToArray();

    graphics.FillRectangle(new SolidBrush(DefaultBezierPath.FillColor), new Rectangle(points[0], new Size(points[2].X - points[0].X, points[2].Y - points[0].Y)));
    graphics.DrawPolygon(LinesPen, points);

    // Optionally, draw the vertices as small circles for visual feedback
    vertices.ForEach(v =>
    {
      // Call the Draw method of each vertex to draw it
      v.Draw(graphics);
    });

    canvas.Invalidate();
  }
}
