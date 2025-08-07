using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Extensions;
using TNT.Drawing.Model;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects;

/// <summary>
/// Represents a line on the <see cref="Canvas"/>
/// </summary>
public class BezierPath : CanvasObject
{
  private readonly Centroid _centroid = new Centroid();
  private static readonly Pen _ControlPointConnectorPen = new Pen(Color.FromArgb(100, Color.Black), 1);
  private static readonly Pen _OutlinePen = new Pen(Color.Black, 1);

  /// <summary>
  /// Indicates the width of the <see cref="BezierPath"/>
  /// </summary>
  public int Width { get => Get(1); set => Set(value); }

  /// <summary>
  /// Represents the <see cref="LineColor"/> as an ARGB value so that it can be persisted
  /// </summary>
  public int LineARGB { get => this.LineColor.ToArgb(); set => this.LineColor = Color.FromArgb(value); }

  /// <summary>
  /// Indicates the <see cref="LineColor"/> of the <see cref="BezierPath"/>
  /// </summary>
  [DisplayName("Line Color")]
  public Color LineColor { get => Get(Color.Blue); set => Set(value); }

  public int FillARGB { get => FillColor.ToArgb(); set => FillColor = Color.FromArgb(value); }

  /// <summary>
  /// Indicates the fill color of the <see cref="BezierPath"/>. This color is used to fill the interior of the path if applicable.
  /// </summary>
  [DisplayName("Fill Color")]
  public Color FillColor { get => Get(Color.Transparent); set => Set(value); }

  /// <summary>
  /// Indicates the <see cref="DashStyle"/> of the <see cref="BezierPath"/>
  /// </summary>
  [DisplayName("Line Style")]
  public DashStyle LineStyle { get => Get(DashStyle.Solid); set => Set(value); }

  /// <summary>
  /// Indicates whether the Bezier path is a closed figure.
  /// When true, the path is closed and can be filled; when false, it is open.
  /// </summary>
  private bool IsClosedPath = false;

  /// <summary>
  /// The <see cref="List{CanvasPoint}"/> represented by this <see cref="BezierPath"/>
  /// </summary>
  protected List<CanvasPoint> CanvasPoints { get; set; } = new List<CanvasPoint>();

  /// <summary>
  /// Creates the <see cref="GraphicsPath"/> represented by <see cref="CanvasPoints"/>
  /// </summary>
  protected virtual GraphicsPath Path
  {
    get
    {
      var path = new GraphicsPath();
      var points = CanvasPoints.Select(v => v.ToPoint).ToArray();
      path.AddBeziers(points);

      if (IsClosedPath) path.CloseFigure();

      return path;
    }
  }

  /// <summary>
  /// Needed for deserialization so that set gets called. 
  /// </summary>
  public CanvasPoint[] PointsArray
  {
    get => CanvasPoints.ToArray();
    set
    {
      CanvasPoints = value.ToList();
      var ctrlPoints = CanvasPoints.ToList().FindAll(p => p is ControlPoint);
      var vertices = CanvasPoints.ToList().FindAll(p => p is Vertex);
    }
  }

  /// <summary>
  /// Represents the <see cref="OutlinePen"/> used when generating this <see cref="BezierPath"/>
  /// </summary>
  private Pen OutlinePen
  {
    get
    {
      _OutlinePen.Color = LineColor;
      _OutlinePen.Width = Width;
      _OutlinePen.DashStyle = LineStyle;
      return _OutlinePen;
    }
  }

  /// <summary>
  /// Gets or sets whether the BezierPath is selected.
  /// When unselected, all points and the centroid are also deselected.
  /// </summary>
  public override bool IsSelected
  {
    get => base.IsSelected;
    set
    {
      // Make sure all selected points are deselected when the path is unselected
      if (!value)
      {
        CanvasPoints.ForEach(p => p.IsSelected = value);
        _centroid.IsSelected = value;
      }
      base.IsSelected = value;
    }
  }

  /// <summary>
  /// Copy constructor
  /// </summary>
  public BezierPath(BezierPath path) : this()
  {
    Width = path.Width;
    LineColor = path.LineColor;
    FillColor = path.FillColor;
    LineStyle = path.LineStyle;
  }

  /// <summary>
  /// Default constructor
  /// </summary>
  public BezierPath()
  {
    // Subscribe to base property changed event
    base.OnPropertyChanged += (propertyName, value) =>
    {
      if (propertyName == nameof(RotationAngle) && value is double angle && Math.Abs(angle) > 0.0001)
      {
        RotateBy(angle, Keys.None);

        // Reset the rotation angle to 0 after transformation
        base.Set(0.0, nameof(RotationAngle));
      }

      GetCentroidPosition()?.Also(p => _centroid.MoveTo(p, Keys.None, true));
    };
  }

  /// <summary>
  /// Adds a <see cref="Vertex"/> to this line
  /// </summary>
  virtual public void AddVertex(Vertex vertex)
  {
    vertex.OnMoved = OnMoved;

    if (CanvasPoints.Count > 0)
    {
      (CanvasPoints.Last(p => p is Vertex) as Vertex)?.Also(p1 =>
      {
        var c1 = new ControlPoint(p1.ToPoint) { OnMoved = OnMoved, IsVisible = IsControlPointVisible };
        var c2 = new ControlPoint(vertex.ToPoint) { OnMoved = OnMoved, IsVisible = IsControlPointVisible };

        CanvasPoints.Add(c1);
        CanvasPoints.Add(c2);
      });
    }

    CanvasPoints.Add(vertex);

    IsClosedPath = CanvasPoints.FindCoincident(vertex) != null;

    TNTLogger.Info($"Adding vertex {vertex} to BezierPath. IsClosedPath: {IsClosedPath}");
  }

  /// <summary>
  /// Indicates if <paramref name="ctrlPoint"/> is visible
  /// </summary>
  /// <returns>True if <paramref name="ctrlPoint"/> is visible, false otherwise</returns>
  private bool IsControlPointVisible(ControlPoint ctrlPoint)
  {
    // Find the vertex next to control point
    var adjacent = CanvasPoints.AdjacentTo(ctrlPoint);
    return adjacent.Find(p => p is Vertex)?.Let(vertex => !(vertex.X == ctrlPoint.X && vertex.Y == ctrlPoint.Y)) ?? false;
  }

  /// <summary>
  /// Called when the <paramref name="canvasPoint"/> is moved
  /// Calculates the angle from the previous location of the canvasPoint (using dx and dy) and the current location to the _centroid.
  /// </summary>
  private void OnMoved(CanvasPoint canvasPoint, int dx, int dy, Keys modifierKeys)
  {

    if (_centroid.IsSelected)
    {
      // Calculate angle from previous and current location to the centroid
      var centroidPos = GetCentroidPosition();

      if (centroidPos != null)
      {
        Point prevLocation = new Point(canvasPoint.X - dx, canvasPoint.Y - dy);
        Point currLocation = canvasPoint.ToPoint;
        int cx = centroidPos.Value.X;
        int cy = centroidPos.Value.Y;
        // Vectors from centroid to previous and current locations
        double v1x = prevLocation.X - cx;
        double v1y = prevLocation.Y - cy;
        double v2x = currLocation.X - cx;
        double v2y = currLocation.Y - cy;

        // Calculate angle between vectors
        double dot = v1x * v2x + v1y * v2y;
        double det = v1x * v2y - v1y * v2x;
        double angleRadians = Math.Atan2(det, dot);
        double angleDegrees = angleRadians * (180.0 / Math.PI);

        // angleDegrees is the signed angle from previous to current location, relative to centroid
        // You can use angleDegrees as needed (e.g., log, store, etc.)
        TNTLogger.Info($"Angle from previous to current location to centroid: {angleDegrees:F2} degrees");

        canvasPoint.MoveTo(prevLocation, modifierKeys, true);
        //CanvasPoints.AdjacentTo(canvasPoint).ForEach(cp=> cp.MoveTo(prevLocation, modifierKeys));
        RotateBy(angleDegrees, modifierKeys);
      }
      return;
    }

    if (canvasPoint is ControlPoint ctrlPoint)
    {
      if (modifierKeys.ContainsAll(Keys.Shift))
      {
        // Find the Vertex adjacent to canvasPoint
        var ctrlPointVertex = CanvasPoints.AdjacentTo(ctrlPoint).FirstOrDefault(p => p is Vertex) as Vertex;
        if (ctrlPointVertex == null) return;

        // Get the opposite control point
        var oppositeVertex = CanvasPoints.FindCoincident(ctrlPointVertex);
        var oppositeCtrlPoint = CanvasPoints.AdjacentTo(ctrlPointVertex).FirstOrDefault(p => p is ControlPoint && p != ctrlPoint) as ControlPoint
          ?? oppositeVertex?.Let(vertex => CanvasPoints.AdjacentTo(vertex).FirstOrDefault());
        ;
        if (oppositeCtrlPoint == null) return;

        var offset = ctrlPointVertex.ToPoint.Subtract(ctrlPoint.ToPoint);
        var newPoint = ctrlPointVertex.ToPoint.Add(offset);
        oppositeCtrlPoint?.MoveTo(newPoint, modifierKeys, true);

      }
    }
    else if (canvasPoint is Vertex vertex)
    {
      CanvasPoints.AdjacentTo(vertex).ForEach(ctrlPoint => ctrlPoint.MoveBy(dx, dy, Keys.None, true));

      if (IsClosedPath && CanvasPoints.IsFirstOrLast(vertex))
      {
        var coincidentPoint = CanvasPoints.FirstOrDefault() == vertex ? CanvasPoints.LastOrDefault() as Vertex : CanvasPoints.FirstOrDefault();
        coincidentPoint?.Also(point =>
          {
            CanvasPoints.AdjacentTo(point).ForEach(ctrlPoint => ctrlPoint.MoveBy(dx, dy, Keys.None, true));
            point.MoveTo(vertex.ToPoint, Keys.None, true);
          }
        );
      }

      CanvasPoints.FindAll(p => p != vertex && p.IsSelected).ForEach(p =>
      {
        CanvasPoints.AdjacentTo(p).ForEach(cp => cp.MoveBy(dx, dy, Keys.None, true));
        p.MoveBy(dx, dy, modifierKeys, true);
      });
    }

    GetCentroidPosition()?.Also(p => _centroid.MoveTo(p, Keys.None, false));
  }

  /// <summary>
  /// Removes a <see cref="Vertex"/> from this <see cref=" BezierPath"/>
  /// </summary>
  public void RemoveVertex(Vertex vertex)
  {
    var vertexIndex = CanvasPoints.IndexOf(vertex);
    var count = vertexIndex == 0 || vertexIndex == CanvasPoints.Count - 1 ? 2 : 3;

    // Remove vertex and associated ControlPoints
    CanvasPoints.RemoveRange(Math.Max(0, vertexIndex - 1), count);

    // Remove ControlPoints that might be at the start or end that are no longer needed
    var orphanedCtrlPoint = CanvasPoints.FirstOrDefault() as ControlPoint ?? CanvasPoints.LastOrDefault() as ControlPoint;
    orphanedCtrlPoint?.Also(p => CanvasPoints.Remove(p));
  }

  public override MouseDownResponse OnMouseDown(Point location, Keys modifierKeys)
  {
    var response = base.OnMouseDown(location, modifierKeys) with { HitObject = IsMouseOver(location, modifierKeys) ? this : null };
    CanvasPoint? innerHitObject = null;

    if (IsSelected)
    {
      Centroid? hitCentroid = _centroid.MouseOver(location, modifierKeys).HitObject as Centroid;
      Vertex? hitVertex = CanvasPoints.FirstOrDefault(p => p is Vertex && p.MouseOver(location, modifierKeys).HitObject != null) as Vertex;
      ControlPoint? hitCtrlPoint = CanvasPoints.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, modifierKeys).HitObject != null) as ControlPoint;

      CanvasPoint? deletablePoint = hitVertex as CanvasPoint ?? hitCtrlPoint;

      if (hitCentroid != null)
      {
        CanvasPoints.ForEach(p => p.IsSelected = false);
        hitCentroid.IsSelected = !hitCentroid.IsSelected;
        innerHitObject = hitCentroid.IsSelected ? hitCentroid : null;
      }
      else if (modifierKeys == Keys.Shift && hitCtrlPoint != null)
      {
        hitCtrlPoint.IsSelected = true;
        innerHitObject = hitCtrlPoint.IsSelected ? hitCtrlPoint : null;
      }
      else if (modifierKeys == (Keys.Control | Keys.Shift) && deletablePoint != null)
      {
        DeletePoint(deletablePoint);
        CanvasPoints.ForEach(p => p.IsSelected = false);
        response = response with { AllowMove = false };
      }
      else if (modifierKeys == (Keys.Control | Keys.Shift) && deletablePoint == null)
      {
        TryAddVertex(location);
        CanvasPoints.ForEach(p => p.IsSelected = false);
        response = response with { AllowMove = false };
      }
      else if (modifierKeys == Keys.Control && hitVertex != null)
      {
        hitVertex.IsSelected = !hitVertex.IsSelected;
        innerHitObject = hitVertex.IsSelected ? hitVertex : null;
      }
      else if (hitVertex != null)
      {
        CanvasPoints.ForEach(p => p.IsSelected = false);
        //hitVertex.IsSelected = true;
        innerHitObject = hitVertex;//.IsSelected ? hitVertex : null;
      }
      else if (hitCtrlPoint != null)
      {
        hitCtrlPoint.IsSelected = !hitCtrlPoint.IsSelected;
        innerHitObject = hitCtrlPoint.IsSelected ? hitCtrlPoint : null;
      }
    }

    return response with { InnerHitObject = innerHitObject };
  }

  /// <summary>
  /// Tries to add a <see cref="Vertex"/> at <paramref name="location"/> from <see cref="BezierPath"/>
  /// </summary>
  private void TryAddVertex(Point location)
  {
    var path = new GraphicsPath();
    var pen = new Pen(Color.Black, 10);
    int? insertIndex = null;

    // Find the line that the mouse is over
    for (var index = 3; insertIndex == null && index < CanvasPoints.Count(); index += 3)
    {
      var points = CanvasPoints.GetRange(index - 3, 4).Select(p => p.ToPoint);
      path.ClearMarkers();
      path.AddBeziers(points.ToArray());

      if (path.IsOutlineVisible(location, pen)) insertIndex = index - 1;
    }

    if (insertIndex != null)
    {
      var vertex = new Vertex(location.X, location.Y) { OnMoved = OnMoved };
      var c1 = new ControlPoint(vertex.ToPoint) { OnMoved = OnMoved, IsVisible = IsControlPointVisible };
      var c2 = new ControlPoint(vertex.ToPoint) { OnMoved = OnMoved, IsVisible = IsControlPointVisible };
      CanvasPoints.InsertRange((int)insertIndex, new List<CanvasPoint>() { c1, vertex, c2 });
    }
  }

  /// <summary>
  /// Deletes the <paramref name="point"/> from <see cref="BezierPath"/>
  /// </summary>
  /// <param name="point"></param>
  private void DeletePoint(CanvasPoint point)
  {
    if (point is Vertex vertex)
    {
      // Only remove if there are two verteces remaining
      if (CanvasPoints.Sum(p => p is Vertex ? 1 : 0) > 2)
      {
        // Remove the vertex
        RemoveVertex(vertex);
      }
    }
    else if (point is ControlPoint ctrlPoint)
    {
      // Reset position of ControlPoint
      var ctrlPointIndex = CanvasPoints.IndexOf(point);
      var canvasPoint = CanvasPoints.ElementAtOrDefault(ctrlPointIndex - 1) as Vertex ?? CanvasPoints.ElementAtOrDefault(ctrlPointIndex + 1) as Vertex;
      (CanvasPoints.ElementAtOrDefault(ctrlPointIndex - 1) as Vertex ?? CanvasPoints.ElementAtOrDefault(ctrlPointIndex + 1) as Vertex)?.Also(vertex => ctrlPoint.MoveTo(vertex.ToPoint, Keys.None));
    }
  }

  /// <summary>
  /// Copies this <see cref="BezierPath"/>
  /// </summary>
  /// <returns></returns>
  public override CanvasObject Clone() => new BezierPath(this);

  /// <summary>
  /// Determines whether the mouse pointer is currently over any part of this <see cref="BezierPath"/>.
  /// Checks if the mouse is over any control point, vertex, the centroid, the outline of the path, or the filled area (if closed).
  /// </summary>
  public override bool IsMouseOver(Point mousePosition, Keys modifierKeys)
  {
    return CanvasPoints.Any(p => (p is ControlPoint || p is Vertex) && p.IsMouseOver(mousePosition, modifierKeys))
        || _centroid.IsMouseOver(mousePosition, modifierKeys)
        || Path.IsOutlineVisible(mousePosition, OUTLINE_PEN_HIT_WITDTH)
        || (IsClosedPath && Path.IsVisible(mousePosition));
  }

  /// <summary>
  /// Gets a <see cref="Cursor"/> representing the state of the <see cref="BezierPath"/> that is represented
  /// by <paramref name="location"/>
  /// </summary>
  /// <returns><see cref="Cursor"/> represented by <paramref name="location"/></returns>
  public override Feedback GetFeedback(Point location, Keys modifierKeys)
  {
    var cursor = Cursors.Hand;
    var hint = "Click to select. CTRL for multiple objects.";

    // Check if over any points
    if (IsSelected)
    {
      cursor = Cursors.Hand;
      hint = "CTRL and SHIFT to add point.";
      var vertex = CanvasPoints.FirstOrDefault(p => p is Vertex && p.MouseOver(location, modifierKeys).HitObject != null);
      var ctrlPoint = CanvasPoints.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, modifierKeys).HitObject != null);
      CanvasPoint? point = vertex ?? ctrlPoint;

      if (point != null)
      {
        if (modifierKeys == (Keys.Control | Keys.Shift))
        {
          cursor = Resources.Cursors.RemovePoint;
          hint = "Click to remove point";
        }
        else if (ctrlPoint != null && modifierKeys == Keys.Shift)
        {
          cursor = Resources.Cursors.AddCurve;
        }
        else
        {
          cursor = Resources.Cursors.MovePoint;
          hint = "CTRL and SHIFT to remove. SHIFT to curve.";
        }
      }
      else
      {
        if (modifierKeys == (Keys.Control | Keys.Shift))
        {
          cursor = Resources.Cursors.AddPoint;
          hint = "Click to add point";
        }
      }
    }
    else if (modifierKeys == Keys.Control)
    {
      hint = "Click to toggle selection.";
    }

    return new Feedback(cursor, hint);
  }

  /// <summary>
  /// Draws <see cref="BezierPath"/>
  /// </summary>
  public override void Draw(Graphics graphics)
  {
    if (IsClosedPath) graphics.FillPath(new SolidBrush(FillColor), Path);
    graphics.DrawPath(OutlinePen, Path);

    if (IsSelected)
    {
      CanvasPoints.ForEach(v => v.Draw(graphics));
      var vertices = CanvasPoints.FindAll(p => p is Vertex);
      vertices.ForEach(v =>
      {
        CanvasPoints.AdjacentTo(v).ForEach(a =>
        {
          graphics.DrawLine(_ControlPointConnectorPen, v.ToPoint, a.ToPoint);
        });
      });

      _centroid.Draw(graphics);
    }
  }

  /// <summary>
  /// Moves the <see cref="BezierPath"/> by the specified <paramref name="dx"/> and <paramref name="dy"/> values.  
  /// This method adjusts the position of all moveable points in the line.  
  /// </summary>
  /// <param name="dx">The horizontal displacement.</param>  
  /// <param name="dy">The vertical displacement.</param>  
  /// <param name="modifierKeys">Modifier keys pressed during the move operation.</param>  
  /// <param name="supressCallback">Indicates whether to suppress the callback during the move operation.</param>  
  public override void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false)
  {
    TNTLogger.Info($"Moving BezierPath by ({dx}, {dy}) with modifier keys: {modifierKeys}");
    if (!_centroid.IsSelected)
    {
      var vertices = CanvasPoints.OfType<Vertex>().Let(points => IsClosedPath ? points.SkipLast(1) : points).ToList();
      vertices.ForEach(v => v?.MoveBy(dx, dy, modifierKeys, supressCallback));
      GetCentroidPosition()?.Also(p => _centroid.MoveTo(p, Keys.None, false));
    }
  }

  /// <summary>
  /// Rotates the <see cref="BezierPath"/> by the specified angle in degrees around its centroid.
  /// </summary>
  /// <param name="dx">The rotation angle in degrees to apply.</param>
  /// <param name="modifierKeys">Modifier keys pressed during the rotation operation.</param>
  public void RotateBy(double dx, Keys modifierKeys)
  {
    // Get the centroid of the path to use as rotation center
    var centroid = GetCentroidPosition();
    if (centroid == null || CanvasPoints.Count == 0)
      return;

    // Convert angle from degrees to radians
    double radians = dx * Math.PI / 180.0;
    int cx = centroid.Value.X;
    int cy = centroid.Value.Y;

    // Calculate rotation matrix values once
    double cos = Math.Cos(radians);
    double sin = Math.Sin(radians);

    // Rotate each point around the centroid
    foreach (var pt in CanvasPoints)
    {
      // Translate point to origin (relative to centroid)
      int dx_point = pt.X - cx;
      int dy_point = pt.Y - cy;

      // Apply rotation
      int rx = (int)Math.Round(dx_point * cos - dy_point * sin) + cx;
      int ry = (int)Math.Round(dx_point * sin + dy_point * cos) + cy;

      // Update point position
      pt.X = rx;
      pt.Y = ry;
    }
  }

  /// <summary>
  /// Aligns <see cref="BezierPath"/> to the <paramref name="alignInterval"/>
  /// </summary>
  public override void Align(int alignInterval) => CanvasPoints.ForEach(p => p.Align(alignInterval));

  /// <summary>
  /// Calculates the centroid (center of mass) of the closed Bezier path in canvas coordinates.
  /// The centroid is computed by flattening the Bezier path into a polygon and applying the standard polygon centroid formula.
  /// Returns null if the path is not closed or has insufficient points to define an area.
  /// </summary>
  public override Point? GetCentroidPosition()
  {
    // 1. Flatten the path to a polygon
    var path = Path;
    path.Flatten(); // Converts curves to line segments

    var points = path.PathPoints;
    if (points.Length < 3)
      return null;

    // 2. Compute the centroid of the polygon
    float area = 0;
    float cx = 0;
    float cy = 0;

    for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
    {
      float xi = points[i].X, yi = points[i].Y;
      float xj = points[j].X, yj = points[j].Y;
      float a = xi * yj - xj * yi;
      area += a;
      cx += (xi + xj) * a;
      cy += (yi + yj) * a;
    }

    area *= 0.5f;
    if (Math.Abs(area) < 1e-5)
      return null; // Degenerate

    cx /= (6 * area);
    cy /= (6 * area);

    return new Point(Convert.ToInt32(cx), Convert.ToInt32(cy));
  }
}