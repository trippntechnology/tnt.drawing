﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Extensions;
using TNT.Drawing.Model;
using TNT.Drawing.Resource;

using CommonsExtensions = TNT.Commons.Extensions;

namespace TNT.Drawing.Objects;

/// <summary>
/// Repesents a line on the <see cref="Canvas"/>
/// </summary>
public class Line : CanvasObject
{
  private List<CanvasPoint> moveablePoints = new();

  private Pen pen = new Pen(Color.Black, 1);

  /// <summary>
  /// Indicates the width of the <see cref="Line"/>
  /// </summary>
  public int Width { get => Get(1); set => Set(value); }

  /// <summary>
  /// Represents the <see cref="Color"/> as an ARGB value so that it can be persisted
  /// </summary>
  public int ColorARGB { get => this.Color.ToArgb(); set => this.Color = Color.FromArgb(value); }

  /// <summary>
  /// Indicates the <see cref="Color"/> of the <see cref="Line"/>
  /// </summary>
  public Color Color { get => Get(Color.Blue); set => Set(value); }

  /// <summary>
  /// Indicates the <see cref="DashStyle"/> of the <see cref="Line"/>
  /// </summary>
  public DashStyle Style { get => Get(DashStyle.Solid); set => Set(value); }

  /// <summary>
  /// The <see cref="List{CanvasPoint}"/> represented by this <see cref="Line"/>
  /// </summary>
  protected List<CanvasPoint> Points { get; set; } = new List<CanvasPoint>();

  /// <summary>
  /// Creates the <see cref="GraphicsPath"/> represented by <see cref="Points"/>
  /// </summary>
  protected GraphicsPath Path
  {
    get
    {
      var path = new GraphicsPath();
      var points = Points.Select(v => v.ToPoint).ToArray();
      path.AddBeziers(points);
      return path;
    }
  }

  /// <summary>
  /// Needed for deserialization so that set gets called. 
  /// </summary>
  public CanvasPoint[] PointsArray
  {
    get => Points.ToArray();
    set
    {
      Points = value.ToList();
      var ctrlPoints = Points.ToList().FindAll(p => p is ControlPoint);
      var vertices = Points.ToList().FindAll(p => p is Vertex);
    }
  }

  /// <summary>
  /// Represents the <see cref="Pen"/> used when generating this <see cref="Line"/>
  /// </summary>
  private Pen Pen
  {
    get
    {
      pen.Color = Color;
      pen.Width = Width;
      pen.DashStyle = Style;
      return pen;
    }
  }

  /// <summary>
  /// Default constructor
  /// </summary>
  public Line() : base() { }

  /// <summary>
  /// Copy constructor
  /// </summary>
  public Line(Line line) : this() { Width = line.Width; Color = line.Color; }

  /// <summary>
  /// Adds a <see cref="Vertex"/> to this line
  /// </summary>
  public void AddVertex(Vertex vertex)
  {
    vertex.OnMoved = OnMoved;

    if (Points.Count > 0)
    {
      (Points.Last(p => p is Vertex) as Vertex)?.Also(p1 =>
      {
        var c1 = new ControlPoint(p1.ToPoint) { OnMoved = OnMoved, IsVisible = IsControlPointVisible };
        var c2 = new ControlPoint(vertex.ToPoint) { OnMoved = OnMoved, IsVisible = IsControlPointVisible };

        Points.Add(c1);
        Points.Add(c2);
      });
    }
    Points.Add(vertex);
  }

  /// <summary>
  /// Indicates if <paramref name="ctrlPoint"/> is visible
  /// </summary>
  /// <returns>True if <paramref name="ctrlPoint"/> is visible, false otherwise</returns>
  private bool IsControlPointVisible(ControlPoint ctrlPoint)
  {
    // Find the vertex next to control point
    var adjacent = Points.AdjacentTo(ctrlPoint);
    return adjacent.Find(p => p is Vertex)?.Let(vertex => !(vertex.X == ctrlPoint.X && vertex.Y == ctrlPoint.Y)) ?? false;
  }

  /// <summary>
  /// Called when the <paramref name="canvasPoint"/> is moved
  /// </summary>
  private void OnMoved(CanvasPoint canvasPoint, int dx, int dy, Keys modifierKeys)
  {
    if (canvasPoint is ControlPoint)
    {
      if (modifierKeys.ContainsAll(Keys.Shift))
      {
        // Find the Vertex adjacent to canvasPoint
        var canvasPointIndex = Points.IndexOf(canvasPoint);
        var adjacentIndex = Points[canvasPointIndex - 1] is Vertex ? canvasPointIndex - 1 : canvasPointIndex + 1;
        //var adjacentVertex = Points.ElementAtOrDefault(adjacentIndex);

        // Get the opposite control point
        var oppositeIndex = adjacentIndex < canvasPointIndex ? adjacentIndex - 1 : adjacentIndex + 1;
        //var oppositeVertex = Points.ElementAtOrDefault(oppositeIndex);

        CommonsExtensions.RunNotNull(Points.ElementAtOrDefault(adjacentIndex), Points.ElementAtOrDefault(oppositeIndex), (adjacentVertex, oppositeVertex) =>
        {
          var offset = adjacentVertex.ToPoint.Subtract(canvasPoint.ToPoint);
          var newPoint = adjacentVertex.ToPoint.Add(offset);
          oppositeVertex?.MoveTo(newPoint, modifierKeys, true);
        });
      }
    }
    else if (canvasPoint is Vertex)
    {
      // Find ControlPoints adjacent to this Vertex
      var vertexIndex = Points.IndexOf(canvasPoint);
      var ctrlPoints = new List<CanvasPoint>();
      ctrlPoints.AddNotNull(Points.ElementAtOrDefault(vertexIndex - 1));
      ctrlPoints.AddNotNull(Points.ElementAtOrDefault(vertexIndex + 1));
      ctrlPoints.ForEach(cp => cp.MoveBy(dx, dy, Keys.None, true));
    }
  }

  /// <summary>
  /// Removes a <see cref="Vertex"/> from this <see cref=" Line"/>
  /// </summary>
  public void RemoveVertex(Vertex vertex)
  {
    var vertexIndex = Points.IndexOf(vertex);
    var count = vertexIndex == 0 || vertexIndex == Points.Count - 1 ? 2 : 3;

    // Remove vertex and associated ControlPoints
    Points.RemoveRange(Math.Max(0, vertexIndex - 1), count);

    // Remove ControlPoints that might be at the start or end that are no longer needed
    var orphanedCtrlPoint = Points.FirstOrDefault() as ControlPoint ?? Points.LastOrDefault() as ControlPoint;
    orphanedCtrlPoint?.Also(p => Points.Remove(p));
  }

  /// <summary>
  /// Handles the mouse down event for the <see cref="Line"/> object.
  /// This method determines the interaction with the line or its points based on the mouse location
  /// and modifier keys pressed. It supports adding, deleting, selecting, and moving vertices or control points.
  /// </summary>
  /// <param name="location">The location of the mouse pointer.</param>
  /// <param name="modifierKeys">The modifier keys pressed during the event.</param>
  /// <returns>A <see cref="MouseDownResponse"/> object indicating the result of the mouse down event.</returns>
  public override MouseDownResponse OnMouseDown(Point location, Keys modifierKeys)
  {
    var response = new MouseDownResponse(this, null, true);

    if (IsSelected)
    {
      // Check if the mouse is over any of the points
      var hitVertex = Points.FirstOrDefault(p => p is Vertex && p.MouseOver(location, modifierKeys).HitObject != null) as Vertex;
      ControlPoint? hitCtrlPoint = Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, modifierKeys).HitObject != null) as ControlPoint;

      CanvasPoint? hitPoint = hitVertex ?? hitCtrlPoint as CanvasPoint;

      if (hitPoint == null && modifierKeys.ContainsAll(Keys.Control, Keys.Shift))
      {
        // Add vertex
        TryAddVertex(location);
        moveablePoints.Clear();
        moveablePoints.AddRange(Points.FindAll(p => p is Vertex));
        response = response with { AllowMove = false };
      }
      else if (hitPoint != null && modifierKeys.ContainsAll(Keys.Control, Keys.Shift))
      {
        // Delete vertex
        DeletePoint(hitPoint);
        moveablePoints.Clear();
        moveablePoints.AddRange(Points.FindAll(p => p is Vertex));
        response = response with { AllowMove = false };
      }
      else if (hitVertex != null && modifierKeys.ContainsAll(Keys.Control))
      {
        // Select/unselect vertex
        if (!hitVertex.IsSelected && moveablePoints.Contains(hitVertex)) moveablePoints.Clear();

        hitVertex.IsSelected = !hitVertex.IsSelected;
        if (hitVertex.IsSelected) moveablePoints.Add(hitVertex); else moveablePoints.Remove(hitVertex);
        response = response with { ChildHitObject = hitVertex, AllowMove = false };
      }
      else if (hitCtrlPoint != null && modifierKeys.ContainsAll(Keys.Shift))
      {
        moveablePoints.ForEach(v => v.IsSelected = false);
        moveablePoints.Clear();
        moveablePoints.Add(hitCtrlPoint);
      }
      else if (hitVertex != null && !hitVertex.IsSelected)
      {
        moveablePoints.ForEach(v => v.IsSelected = false);
        moveablePoints.Clear();
        hitVertex.IsSelected = true;
        moveablePoints.Add(hitVertex);
      }
      else if (hitPoint != null && !moveablePoints.Contains(hitPoint))
      {
        // Select single vertex
        moveablePoints.ForEach(v => v.IsSelected = false);
        hitPoint.IsSelected = true;
        moveablePoints.Clear();
        moveablePoints.Add(hitPoint);
        response = response with { ChildHitObject = hitPoint };
      }
      else if (hitPoint == null || !moveablePoints.Contains(hitPoint))
      {
        // Select all vertices
        moveablePoints.ForEach(v => v.IsSelected = false);
        moveablePoints.Clear();
        moveablePoints.AddRange(Points.FindAll(p => p is Vertex));
      }
    }
    else
    {
      moveablePoints.ForEach(v => v.IsSelected = false);
      moveablePoints.Clear();
      moveablePoints.AddRange(Points.FindAll(p => p is Vertex));
    }

    return response;
  }

  /// <summary>
  /// Tries to add a <see cref="Vertex"/> at <paramref name="location"/> from <see cref="Line"/>
  /// </summary>
  private void TryAddVertex(Point location)
  {
    var path = new GraphicsPath();
    var pen = new Pen(Color.Black, 10);
    int? insertIndex = null;

    // Find the line that the mouse is over
    for (var index = 3; insertIndex == null && index < Points.Count(); index += 3)
    {
      var points = Points.GetRange(index - 3, 4).Select(p => p.ToPoint);
      path.ClearMarkers();
      path.AddBeziers(points.ToArray());

      if (path.IsOutlineVisible(location, pen)) insertIndex = index - 1;
    }

    if (insertIndex != null)
    {
      var vertex = new Vertex(location.X, location.Y) { OnMoved = OnMoved };
      var c1 = new ControlPoint(vertex.ToPoint) { OnMoved = OnMoved, IsVisible = IsControlPointVisible };
      var c2 = new ControlPoint(vertex.ToPoint) { OnMoved = OnMoved, IsVisible = IsControlPointVisible };
      Points.InsertRange((int)insertIndex, new List<CanvasPoint>() { c1, vertex, c2 });
    }
  }

  /// <summary>
  /// Deletes the <paramref name="point"/> from <see cref="Line"/>
  /// </summary>
  /// <param name="point"></param>
  private void DeletePoint(CanvasPoint point)
  {
    if (point is Vertex vertex)
    {
      // Only remove if there are two verteces remaining
      if (Points.Sum(p => p is Vertex ? 1 : 0) > 2)
      {
        // Remove the vertex
        RemoveVertex(vertex);
      }
    }
    else if (point is ControlPoint ctrlPoint)
    {
      // Reset position of ControlPoint
      var ctrlPointIndex = Points.IndexOf(point);
      var canvasPoint = Points.ElementAtOrDefault(ctrlPointIndex - 1) as Vertex ?? Points.ElementAtOrDefault(ctrlPointIndex + 1) as Vertex;
      (Points.ElementAtOrDefault(ctrlPointIndex - 1) as Vertex ?? Points.ElementAtOrDefault(ctrlPointIndex + 1) as Vertex)?.Also(vertex => ctrlPoint.MoveTo(vertex.ToPoint, Keys.None));
    }
  }

  /// <summary>
  /// Copies this <see cref="Line"/>
  /// </summary>
  /// <returns></returns>
  public override CanvasObject Copy() => new Line(this);

  /// <summary>
  /// Checks if <paramref name="mousePosition"/> is over any part of this <see cref="Line"/> and return the 
  /// <see cref="CanvasObject"/> within the line that it is over
  /// </summary>
  /// <returns><see cref="CanvasObject"/> within the line that it is over</returns>
  public override MouseOverResponse MouseOver(Point mousePosition, Keys modifierKeys)
  {
    CanvasObject? hitObject = null;

    if (Points.Count > 3)
    {
      // Check if over this line
      hitObject = Path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) ? this : null;

      if (hitObject == null)
      {
        // Check if over any points that might be outside of the line
        hitObject = Points.FirstOrDefault(p => p.MouseOver(mousePosition, modifierKeys).HitObject != null) != null ? this : null;
      }
    }

    return hitObject?.Let(it => new MouseOverResponse(it)) ?? MouseOverResponse.Default;
  }

  /// <summary>
  /// Gets a <see cref="Cursor"/> representing the state of the <see cref="Line"/> that is represented
  /// by <paramref name="location"/>
  /// </summary>
  /// <returns><see cref="Cursor"/> represented by <paramref name="location"/></returns>
  public override Feedback GetFeedback(Point location, Keys modifierKeys)
  {
    var cursor = Cursors.Hand;
    var hint = "Click to select. SHIFT for multiple objects.";

    // Check if over any points
    if (IsSelected)
    {
      cursor = Cursors.Hand;
      hint = "CTRL and SHIFT to add point.";
      var vertex = Points.FirstOrDefault(p => p is Vertex && p.MouseOver(location, modifierKeys).HitObject != null);
      var ctrlPoint = Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, modifierKeys).HitObject != null);
      CanvasPoint? point = vertex ?? ctrlPoint;

      if (point != null)
      {
        if (modifierKeys.ContainsAll(Keys.Control, Keys.Shift))
        {
          cursor = Resources.Cursors.RemovePoint;
        }
        else if (ctrlPoint != null && modifierKeys.ContainsAll(Keys.Shift))
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
        if (modifierKeys.ContainsAll(Keys.Control, Keys.Shift))
        {
          cursor = Resources.Cursors.AddPoint;
          hint = "Click to add point";
        }
      }
    }

    return new Feedback(cursor, hint);
  }

  /// <summary>
  /// Draws <see cref="Line"/>
  /// </summary>
  public override void Draw(Graphics graphics)
  {
    graphics.DrawPath(Pen, Path);
    if (IsSelected)
    {
      Pen pen = new Pen(Color.FromArgb(100, Color.Black));
      Points.ForEach(v => v.Draw(graphics));
      var vertices = Points.FindAll(p => p is Vertex);
      vertices.ForEach(v =>
      {
        Points.AdjacentTo(v).ForEach(a =>
        {
          graphics.DrawLine(pen, v.ToPoint, a.ToPoint);
        });
      });
    }
  }

  /// <summary>
  /// Moves the <see cref="Line"/> by the specified <paramref name="dx"/> and <paramref name="dy"/> values.  
  /// This method adjusts the position of all moveable points in the line.  
  /// </summary>
  /// <param name="dx">The horizontal displacement.</param>  
  /// <param name="dy">The vertical displacement.</param>  
  /// <param name="modifierKeys">Modifier keys pressed during the move operation.</param>  
  /// <param name="supressCallback">Indicates whether to suppress the callback during the move operation.</param>  
  public override void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false) => moveablePoints.ForEach(v => v.MoveBy(dx, dy, modifierKeys));

  /// <summary>
  /// Aligns <see cref="Line"/> to the <paramref name="alignInterval"/>
  /// </summary>
  public override void Align(int alignInterval) => Points.ForEach(p => p.Align(alignInterval));
}