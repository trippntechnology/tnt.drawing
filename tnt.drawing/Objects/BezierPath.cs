﻿using System;
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
/// Repesents a line on the <see cref="Canvas"/>
/// </summary>
public class BezierPath() : CanvasObject
{
  private List<CanvasPoint> moveablePoints = new();

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

  private bool IsClosedPath => CanvasPoints.FirstOrDefault()?.Let(firstPoint => CanvasPoints.FindCoincident(firstPoint) != null) ?? false;

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

  private readonly Pen _ControlPointConnectorPen = new Pen(Color.FromArgb(100, Color.Black), 1);
  private Pen _OutlinePen = new Pen(Color.Black, 1);

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
  /// </summary>
  private void OnMoved(CanvasPoint canvasPoint, int dx, int dy, Keys modifierKeys)
  {
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
    }
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

  /// <summary>
  /// Handles the mouse down event for the <see cref="BezierPath"/> object.
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
      var hitVertex = CanvasPoints.FirstOrDefault(p => p is Vertex && p.MouseOver(location, modifierKeys).HitObject != null) as Vertex;
      ControlPoint? hitCtrlPoint = CanvasPoints.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, modifierKeys).HitObject != null) as ControlPoint;

      CanvasPoint? hitPoint = hitVertex ?? hitCtrlPoint as CanvasPoint;

      if (hitPoint == null && modifierKeys.ContainsAll(Keys.Control, Keys.Shift))
      {
        // Add vertex
        TryAddVertex(location);
        moveablePoints.Clear();
        moveablePoints.AddRange(CanvasPoints.FindAll(p => p is Vertex));
        response = response with { AllowMove = false };
      }
      else if (hitPoint != null && modifierKeys.ContainsAll(Keys.Control, Keys.Shift))
      {
        // Delete vertex
        DeletePoint(hitPoint);
        moveablePoints.Clear();
        moveablePoints.AddRange(CanvasPoints.FindAll(p => p is Vertex));
        response = response with { AllowMove = false };
      }
      else if (hitVertex != null && modifierKeys == Keys.Control)
      {
        var vertices = new List<Vertex>() { hitVertex };
        CanvasPoints.FindCoincident(hitVertex).Also(coincidentPoint =>
        {
          if (coincidentPoint is Vertex coincidentVertex)
          {
            vertices.Add(coincidentVertex);
          }
        });

        // Select/unselect vertex
        vertices.ForEach(v =>
        {
          v.IsSelected = !v.IsSelected;
          if (v.IsSelected) moveablePoints.Add(v); else moveablePoints.Remove(v);
        });

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
        CanvasPoints.FindCoincident(hitVertex)?.Also(coincidentPoint =>
        {
          coincidentPoint.IsSelected = true;
          moveablePoints.Add(coincidentPoint);
        });
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
        moveablePoints.AddRange(CanvasPoints.FindAll(p => p is Vertex));
      }
    }
    else
    {
      moveablePoints.ForEach(v => v.IsSelected = false);
      moveablePoints.Clear();
      moveablePoints.AddRange(CanvasPoints.FindAll(p => p is Vertex));
    }

    return response;
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
  /// Checks if <paramref name="mousePosition"/> is over any part of this <see cref="BezierPath"/> and return the 
  /// <see cref="CanvasObject"/> within the line that it is over
  /// </summary>
  /// <returns><see cref="CanvasObject"/> within the line that it is over</returns>
  public override MouseOverResponse MouseOver(Point mousePosition, Keys modifierKeys)
  {
    CanvasObject? hitObject = null;

    if (CanvasPoints.Count > 3)
    {
      // Check if over this line
      hitObject = Path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) || (IsClosedPath && Path.IsVisible(mousePosition)) ? this : null;

      if (hitObject == null)
      {
        // Check if over any points that might be outside of the line
        hitObject = CanvasPoints.FirstOrDefault(p => p.MouseOver(mousePosition, modifierKeys).HitObject != null) != null ? this : null;
      }
    }

    return hitObject?.Let(it => new MouseOverResponse(it)) ?? MouseOverResponse.Default;
  }

  /// <summary>
  /// Gets a <see cref="Cursor"/> representing the state of the <see cref="BezierPath"/> that is represented
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
  public override void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false) => moveablePoints.ForEach(v => v.MoveBy(dx, dy, modifierKeys));

  /// <summary>
  /// Aligns <see cref="BezierPath"/> to the <paramref name="alignInterval"/>
  /// </summary>
  public override void Align(int alignInterval) => CanvasPoints.ForEach(p => p.Align(alignInterval));
}