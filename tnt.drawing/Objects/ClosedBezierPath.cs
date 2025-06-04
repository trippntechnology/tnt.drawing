using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace TNT.Drawing.Objects;

/// <summary>
/// Represents a closed Bezier path on the <see cref="Canvas"/>, forming a filled shape.
/// </summary>
public class ClosedBezierPath : BezierPath
{
  /// <summary>
  /// Gets or sets the fill color of the closed Bezier path.
  /// </summary>
  public Color FillColor { get => Get(Color.FromArgb(64, Color.Blue)); set => Set(value); }

  public override void AddVertex(Vertex vertex)
  {
    base.AddVertex(vertex);
  }

  /// <summary>
  /// Gets the <see cref="GraphicsPath"/> for this closed Bezier path, ensuring the path is closed.
  /// </summary>
  protected override GraphicsPath Path
  {
    get
    {
      var path = new GraphicsPath();


      // Always ensure a Bezier segment from last to first, synchronizing control points
      if (CanvasPoints.Count > 3)
      {
        // Collect all points for the main path
        var points = CanvasPoints.Select(v => v.ToPoint).ToList();
        path.AddBeziers(points.ToArray());

        // Synchronize closing segment control points
        // Assume Vertex has ControlPoint1 and ControlPoint2 properties (adjust if different)
        var first = CanvasPoints[0];
        var last = CanvasPoints[CanvasPoints.Count - 1];

        // For a cubic Bezier, we need:
        // - Start: last point
        // - Control1: last.ControlPoint2 (outgoing from last)
        // - Control2: first.ControlPoint1 (incoming to first)
        // - End: first point
        // If control points are not available, fall back to the anchor points
        var lastPt = last.ToPoint;
        var firstPt = first.ToPoint;
        var lastCtrl = lastPt;
        var firstCtrl = firstPt;
        var lastType = last.GetType();
        var firstType = first.GetType();
        var cp2Prop = lastType.GetProperty("ControlPoint2");
        var cp1Prop = firstType.GetProperty("ControlPoint1");
        if (cp2Prop != null)
        {
          var val = cp2Prop.GetValue(last);
          if (val is Point p) lastCtrl = p;
        }
        if (cp1Prop != null)
        {
          var val = cp1Prop.GetValue(first);
          if (val is Point p) firstCtrl = p;
        }

        // Only add if the closing segment is not already present
        if (!lastPt.Equals(firstPt) || !lastCtrl.Equals(firstCtrl))
        {
          path.AddBezier(lastPt, lastCtrl, firstCtrl, firstPt);
        }
        path.CloseFigure();
      }
      return path;
    }
  }

  /// <summary>
  /// Draws the closed Bezier path, filling the shape and drawing the outline.
  /// </summary>
  /// <param name="graphics">The graphics context to draw on.</param>
  public override void Draw(Graphics graphics)
  {
    if (CanvasPoints.Count > 3)
    {
      using (var brush = new SolidBrush(FillColor))
      {
        graphics.FillPath(brush, Path);
      }
    }
    base.Draw(graphics);
  }
}
