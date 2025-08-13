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
  // Fields
  private readonly Centroid _centroid = new Centroid();
  private static readonly Pen _ControlPointConnectorPen = new Pen(Color.FromArgb(100, Color.Black), 1);
  private static readonly Pen _OutlinePen = new Pen(Color.Black, 1);
  private bool _isClosedPath = false;
  private bool _isMultiSelectMode = false;

  // Properties
  /// <summary>
  /// The <see cref="List{CanvasPoint}"/> represented by this <see cref="BezierPath"/>
  /// </summary>
  protected List<CanvasPoint> CanvasPoints { get; set; } = new List<CanvasPoint>();

  /// <summary>
  /// Represents the <see cref="FillColor"/> as an ARGB value so that it can be persisted
  /// </summary>
  public int FillARGB { get => FillColor.ToArgb(); set => FillColor = Color.FromArgb(value); }

  /// <summary>
  /// Indicates the fill color of the <see cref="BezierPath"/>. This color is used to fill the interior of the path if applicable.
  /// </summary>
  [DisplayName("Fill Color")]
  public Color FillColor { get => Get(Color.Transparent); set => Set(value); }

  /// <summary>
  /// Gets or sets whether the BezierPath is selected.
  /// When unselected, all points and the centroid are also deselected.
  /// </summary>
  public override bool IsSelected
  {
    get => base.IsSelected;
    set
    {
      if (!value)
      {
        CanvasPoints.ForEach(p => p.IsSelected = value);
        _centroid.IsSelected = value;
      }
      base.IsSelected = value;
    }
  }

  /// <summary>
  /// Represents the <see cref="LineColor"/> as an ARGB value so that it can be persisted
  /// </summary>
  public int LineARGB { get => LineColor.ToArgb(); set => LineColor = Color.FromArgb(value); }
  /// <summary>
  /// Indicates the <see cref="LineColor"/> of the <see cref="BezierPath"/>
  /// </summary>
  [DisplayName("Line Color")]
  public Color LineColor { get => Get(Color.Blue); set => Set(value); }

  /// <summary>
  /// Indicates the <see cref="DashStyle"/> of the <see cref="BezierPath"/>
  /// </summary>
  [DisplayName("Line Style")]
  public DashStyle LineStyle { get => Get(DashStyle.Solid); set => Set(value); }

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
  /// Creates the <see cref="GraphicsPath"/> represented by <see cref="CanvasPoints"/>
  /// </summary>
  protected virtual GraphicsPath Path
  {
    get
    {
      var path = new GraphicsPath();
      var points = CanvasPoints.Select(v => v.ToPoint).ToArray();
      path.AddBeziers(points);
      if (_isClosedPath) path.CloseFigure();
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
      // For deserialization; can be extended if needed
    }
  }

  /// <summary>
  /// Indicates the width of the <see cref="BezierPath"/>
  /// </summary>
  public int Width { get => Get(1); set => Set(value); }

  // Constructors
  /// <summary>
  /// Default constructor
  /// </summary>
  public BezierPath()
  {
    base.OnPropertyChanged += (propertyName, value) =>
    {
      if (propertyName == nameof(RotationAngle) && value is double angle && Math.Abs(angle) > 0.0001)
      {
        RotateBy(angle, Keys.None);
        base.Set(0.0, nameof(RotationAngle));
      }
      GetCentroidPosition()?.Also(p => _centroid.MoveTo(p, Keys.None, true));
    };
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

  // Public Methods
  /// <summary>
  /// Adds a <see cref="Vertex"/> to this line
  /// </summary>
  public virtual void AddVertex(Vertex vertex)
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
    _isClosedPath = CanvasPoints.FindCoincident(vertex) != null;
    TNTLogger.Info($"Adding vertex {vertex} to BezierPath. IsClosedPath: {_isClosedPath}");
  }

  /// <summary>
  /// Aligns <see cref="BezierPath"/> to the <paramref name="alignInterval"/>
  /// </summary>
  public override void Align(int alignInterval) => CanvasPoints.ForEach(p => p.Align(alignInterval));

  /// <summary>
  /// Copies this <see cref="BezierPath"/>
  /// </summary>
  public override CanvasObject Clone() => new BezierPath(this);

  /// <summary>
  /// Removes a <see cref="Vertex"/> or <see cref="ControlPoint"/> from this <see cref="BezierPath"/>.
  /// If the point is a vertex and the path is closed, handles coincident points and opens the path if needed.
  /// If the point is a control point, resets its position to the adjacent vertex.
  /// </summary>
  private void DeletePoint(CanvasPoint point)
  {
    if (point is Vertex vertex)
    {
      if (CanvasPoints.Count(p => p is Vertex) > 2)
      {
        // If closed and deleting first or last vertex, remove coincident point and open the path
        if (_isClosedPath && CanvasPoints.IsFirstOrLast(vertex))
        {
          (CanvasPoints.FindCoincident(vertex) as Vertex)?.Also(v => RemoveVertex(v));
          _isClosedPath = false;
        }

        RemoveVertex(vertex);
      }
    }
    else if (point is ControlPoint ctrlPoint)
    {
      var ctrlPointIndex = CanvasPoints.IndexOf(point);
      (CanvasPoints.ElementAtOrDefault(ctrlPointIndex - 1) as Vertex ?? CanvasPoints.ElementAtOrDefault(ctrlPointIndex + 1) as Vertex)?.Also(vertex => ctrlPoint.MoveTo(vertex.ToPoint, Keys.None));
    }
  }

  /// <summary>
  /// Draws <see cref="BezierPath"/>
  /// </summary>
  public override void Draw(Graphics graphics)
  {
    if (_isClosedPath) graphics.FillPath(new SolidBrush(FillColor), Path);
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
  /// Calculates the centroid (center of mass) of the closed Bezier path in canvas coordinates.
  /// </summary>
  public override Point? GetCentroidPosition()
  {
    var path = Path;
    path.Flatten();
    var points = path.PathPoints;
    if (points.Length < 3)
      return null;
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
      return null;
    cx /= (6 * area);
    cy /= (6 * area);
    return new Point(Convert.ToInt32(cx), Convert.ToInt32(cy));
  }

  /// <summary>
  /// Gets a <see cref="Cursor"/> representing the state of the <see cref="BezierPath"/> that is represented by <paramref name="location"/>
  /// </summary>
  public override Feedback GetFeedback(Point location, Keys modifierKeys)
  {
    var cursor = Cursors.Hand;
    var hint = "Click to select. CTRL for multiple objects.";
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
  /// Determines whether this <see cref="BezierPath"/> intersects with the specified <see cref="Region"/>.
  /// For open paths, returns true if any vertex is inside the region.
  /// For closed paths, returns true if the region and the path's region intersect.
  /// </summary>
  /// <param name="region">The <see cref="Region"/> to test for intersection.</param>
  /// <returns><c>true</c> if the path intersects with the region; otherwise, <c>false</c>.</returns>
  public override bool IntersectsWith(Region region)
  {
    if (!_isClosedPath)
    {
      // For open paths, check if any vertex is inside the region
      foreach (var vertex in CanvasPoints.OfType<Vertex>())
      {
        if (region.IsVisible(vertex.ToPoint))
          return true;
      }
      return false;
    }
    else
    {
      // For closed paths, check if the region and the path's region intersect
      using (var pathRegion = new Region(Path))
      {
        Region intersectRegion = region.Clone();
        intersectRegion.Intersect(pathRegion);
        bool result = !intersectRegion.IsEmpty(Graphics.FromHwnd(IntPtr.Zero));
        intersectRegion.Dispose();
        return result;
      }
    }
  }

  /// <summary>
  /// Indicates if <paramref name="ctrlPoint"/> is visible
  /// </summary>
  private bool IsControlPointVisible(ControlPoint ctrlPoint)
  {
    var adjacent = CanvasPoints.AdjacentTo(ctrlPoint);
    return adjacent.Find(p => p is Vertex)?.Let(vertex => !(vertex.X == ctrlPoint.X && vertex.Y == ctrlPoint.Y)) ?? false;
  }

  /// <summary>
  /// Determines whether the mouse pointer is currently over any part of this <see cref="BezierPath"/>.
  /// </summary>
  public override bool IsMouseOver(Point mousePosition, Keys modifierKeys)
  {
    return CanvasPoints.Any(p => (p is ControlPoint || p is Vertex) && p.IsMouseOver(mousePosition, modifierKeys))
        || _centroid.IsMouseOver(mousePosition, modifierKeys)
        || Path.IsOutlineVisible(mousePosition, OUTLINE_PEN_HIT_WITDTH)
        || (_isClosedPath && Path.IsVisible(mousePosition));
  }

  /// <summary>
  /// Moves the <see cref="BezierPath"/> by the specified <paramref name="dx"/> and <paramref name="dy"/> values.  
  /// </summary>
  public override void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false)
  {
    TNTLogger.Info($"Moving BezierPath by ({dx}, {dy}) with modifier keys: {modifierKeys}");
    if (!_centroid.IsSelected)
    {
      var vertices = CanvasPoints.OfType<Vertex>().Let(points => _isClosedPath ? points.SkipLast(1) : points).ToList();
      vertices.ForEach(v => v?.MoveBy(dx, dy, modifierKeys, supressCallback));
      GetCentroidPosition()?.Also(p => _centroid.MoveTo(p, Keys.None, false));
    }
  }

  /// <summary>
  /// Called when a button press event occurs over an object.  
  /// This method can be overridden to provide custom behavior when the mouse button is pressed.  
  /// </summary>
  public override MouseDownResponse OnMouseDown(Point location, Keys modifierKeys)
  {
    var response = base.OnMouseDown(location, modifierKeys) with { HitObject = IsMouseOver(location, modifierKeys) ? this : null };

    if (IsSelected)
    {
      Centroid? hitCentroid = _centroid.MouseOver(location, modifierKeys).HitObject as Centroid;
      Vertex? hitVertex = CanvasPoints.FirstOrDefault(p => p is Vertex && p.MouseOver(location, modifierKeys).HitObject != null) as Vertex;
      ControlPoint? hitCtrlPoint = CanvasPoints.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, modifierKeys).HitObject != null) as ControlPoint;
      CanvasPoint? deletablePoint = hitVertex as CanvasPoint ?? hitCtrlPoint;

      if (_isMultiSelectMode && hitVertex == null)
      {
        CanvasPoints.OfType<Vertex>().ToList().ForEach(v => IsSelected = false);
      }

      if (hitCentroid != null)
      {
        // If the centroid is hit, toggle its selection and deselect all points
        CanvasPoints.ForEach(p => p.IsSelected = false);
        hitCentroid.IsSelected = !hitCentroid.IsSelected;
        response = response with { InnerHitObject = hitCentroid.IsSelected ? hitCentroid : null, AllowMove = false };
      }
      else if (modifierKeys == Keys.Shift && hitCtrlPoint != null)
      {
        // If Shift is held and a control point is hit, select it
        hitCtrlPoint.IsSelected = true;
        response = response with { InnerHitObject = hitCtrlPoint.IsSelected ? hitCtrlPoint : null };
      }
      else if (modifierKeys == (Keys.Control | Keys.Shift) && deletablePoint != null)
      {
        // If Ctrl+Shift is held and a deletable point is hit, delete it and clear selection
        DeletePoint(deletablePoint);
        CanvasPoints.ForEach(p => p.IsSelected = false);
        response = response with { AllowMove = false };
      }
      else if (modifierKeys == (Keys.Control | Keys.Shift) && deletablePoint == null)
      {
        // If Ctrl+Shift is held and no deletable point is hit, try to add a vertex at the location
        TryAddVertex(location);
        CanvasPoints.ForEach(p => p.IsSelected = false);
        response = response with { AllowMove = false };
      }
      else if (modifierKeys == Keys.Control && hitVertex != null)
      {
        // If Ctrl is held and a vertex is hit, toggle its selection
        hitVertex.IsSelected = !hitVertex.IsSelected;

        // If the vertex is first or last and has a coincident vertex, match selection state
        if (CanvasPoints.IsFirstOrLast(hitVertex))
        {
          (CanvasPoints.FindCoincident(hitVertex) as Vertex)?.Also(v => v.IsSelected = hitVertex.IsSelected);
        }

        response = response with { InnerHitObject = hitVertex, AllowMove = false };
      }
      else if (hitVertex != null)
      {
        // If a vertex is hit (no modifiers), select only it
        if (!hitVertex.IsSelected) CanvasPoints.ForEach(p => p.IsSelected = false);
        response = response with { InnerHitObject = hitVertex };
      }
      else if (hitCtrlPoint != null)
      {
        // If a control point is hit (no modifiers), set it as the inner hit object in the response
        response = response with { InnerHitObject = hitCtrlPoint };
      }

      // Set _isMultiSelectMode to true if any vertex is selected, otherwise false
      _isMultiSelectMode = CanvasPoints.OfType<Vertex>().Any(v => v.IsSelected);
    }

    return response;
  }

  /// <summary>
  /// Called when the <paramref name="canvasPoint"/> is moved
  /// </summary>
  private void OnMoved(CanvasPoint canvasPoint, int dx, int dy, Keys modifierKeys)
  {
    if (_centroid.IsSelected)
    {
      var centroidPos = GetCentroidPosition();
      if (centroidPos != null)
      {
        Point prevLocation = new Point(canvasPoint.X - dx, canvasPoint.Y - dy);
        Point currLocation = canvasPoint.ToPoint;
        int cx = centroidPos.Value.X;
        int cy = centroidPos.Value.Y;
        double v1x = prevLocation.X - cx;
        double v1y = prevLocation.Y - cy;
        double v2x = currLocation.X - cx;
        double v2y = currLocation.Y - cy;
        double dot = v1x * v2x + v1y * v2y;
        double det = v1x * v2y - v1y * v2x;
        double angleRadians = Math.Atan2(det, dot);
        double angleDegrees = angleRadians * (180.0 / Math.PI);
        TNTLogger.Info($"Angle from previous to current location to centroid: {angleDegrees:F2} degrees");
        canvasPoint.MoveTo(prevLocation, modifierKeys, true);
        RotateBy(angleDegrees, modifierKeys);
      }
      return;
    }

    if (canvasPoint is ControlPoint ctrlPoint)
    {
      if (modifierKeys.ContainsAll(Keys.Shift))
      {
        var ctrlPointVertex = CanvasPoints.AdjacentTo(ctrlPoint).FirstOrDefault(p => p is Vertex) as Vertex;
        if (ctrlPointVertex == null) return;
        var oppositeVertex = CanvasPoints.FindCoincident(ctrlPointVertex);
        var oppositeCtrlPoint = CanvasPoints.AdjacentTo(ctrlPointVertex).FirstOrDefault(p => p is ControlPoint && p != ctrlPoint) as ControlPoint
            ?? oppositeVertex?.Let(vertex => CanvasPoints.AdjacentTo(vertex).FirstOrDefault());
        if (oppositeCtrlPoint == null) return;
        var offset = ctrlPointVertex.ToPoint.Subtract(ctrlPoint.ToPoint);
        var newPoint = ctrlPointVertex.ToPoint.Add(offset);
        oppositeCtrlPoint?.MoveTo(newPoint, modifierKeys, true);
      }
    }
    else if (canvasPoint is Vertex vertex)
    {
      // If multi-select mode is active and this vertex is selected, move all other selected vertices
      if (_isMultiSelectMode && vertex.IsSelected)
      {
        foreach (var v in CanvasPoints.OfType<Vertex>().Where(v => v.IsSelected && v != vertex))
        {
          v.MoveBy(dx, dy, modifierKeys, true);
          CanvasPoints.AdjacentTo(v).ForEach(ctrl => ctrl.MoveBy(dx, dy, Keys.None, true));
        }
      }

      CanvasPoints.AdjacentTo(vertex).ForEach(ctrlPoint => ctrlPoint.MoveBy(dx, dy, Keys.None, true));
      if (_isClosedPath && CanvasPoints.IsFirstOrLast(vertex))
      {
        var coincidentPoint = CanvasPoints.FirstOrDefault() == vertex ? CanvasPoints.LastOrDefault() as Vertex : CanvasPoints.FirstOrDefault();
        coincidentPoint?.Also(point =>
        {
          CanvasPoints.AdjacentTo(point).ForEach(ctrlPoint => ctrlPoint.MoveBy(dx, dy, Keys.None, true));
          point.MoveTo(vertex.ToPoint, Keys.None, true);
        });
      }
      else if (CanvasPoints.IsFirstOrLast(vertex))
      {
        _isClosedPath = CanvasPoints.FindCoincident(vertex) != null;
      }
    }

    GetCentroidPosition()?.Also(p => _centroid.MoveTo(p, Keys.None, false));
  }

  /// <summary>
  /// Removes the specified <see cref="Vertex"/> and its associated <see cref="ControlPoint"/>s from this <see cref="BezierPath"/>.
  /// If the vertex is at the start or end of the path, removes 2 points (the vertex and one control point);
  /// otherwise, removes 3 points (the vertex and its two adjacent control points).
  /// Also removes any orphaned control point at the start or end of the path if present.
  /// </summary>
  public void RemoveVertex(Vertex vertex)
  {
    var vertexIndex = CanvasPoints.IndexOf(vertex);
    var count = vertexIndex == 0 || vertexIndex == CanvasPoints.Count - 1 ? 2 : 3;
    CanvasPoints.RemoveRange(Math.Max(0, vertexIndex - 1), count);
    var orphanedCtrlPoint = CanvasPoints.FirstOrDefault() as ControlPoint ?? CanvasPoints.LastOrDefault() as ControlPoint;
    orphanedCtrlPoint?.Also(p => CanvasPoints.Remove(p));
  }

  /// <summary>
  /// Rotates the <see cref="BezierPath"/> by the specified angle in degrees around its centroid.
  /// </summary>
  public void RotateBy(double dx, Keys modifierKeys)
  {
    var centroid = GetCentroidPosition();
    if (centroid == null || CanvasPoints.Count == 0)
      return;
    double radians = dx * Math.PI / 180.0;
    int cx = centroid.Value.X;
    int cy = centroid.Value.Y;
    double cos = Math.Cos(radians);
    double sin = Math.Sin(radians);
    foreach (var pt in CanvasPoints)
    {
      int dx_point = pt.X - cx;
      int dy_point = pt.Y - cy;
      int rx = (int)Math.Round(dx_point * cos - dy_point * sin) + cx;
      int ry = (int)Math.Round(dx_point * sin + dy_point * cos) + cy;
      pt.X = rx;
      pt.Y = ry;
    }
  }

  /// <summary>
  /// Tries to add a <see cref="Vertex"/> at <paramref name="location"/> from <see cref="BezierPath"/>
  /// </summary>
  private void TryAddVertex(Point location)
  {
    var path = new GraphicsPath();
    var pen = new Pen(Color.Black, 10);
    int? insertIndex = null;
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
}