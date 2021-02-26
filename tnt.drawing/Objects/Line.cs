using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using TNT.Drawing.Resource;

namespace TNT.Drawing.Objects
{
	/// <summary>
	/// Repesents a line on the <see cref="Canvas"/>
	/// </summary>
	public class Line : CanvasObject
	{
		private CanvasObject activeObject = null;
		private Pen _Pen = new Pen(Color.Black, 1);

		/// <summary>
		/// Indicates the width of the <see cref="Line"/>
		/// </summary>
		public float Width { get; set; } = 1;

		/// <summary>
		/// Represents the <see cref="Color"/> as an ARGB value so that it can be persisted
		/// </summary>
		public int ColorARGB { get => this.Color.ToArgb(); set => this.Color = Color.FromArgb(value); }

		/// <summary>
		/// Indicates the <see cref="Color"/> of the <see cref="Line"/>
		/// </summary>
		[XmlIgnore]
		public Color Color { get; set; } = Color.Blue;

		/// <summary>
		/// Indicates the <see cref="DashStyle"/> of the <see cref="Line"/>
		/// </summary>
		public DashStyle Style { get; set; } = DashStyle.Solid;

		/// <summary>
		/// The <see cref="List{CanvasPoint}"/> represented by this <see cref="Line"/>
		/// </summary>
		[XmlIgnore]
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
		public Pen Pen
		{
			get
			{
				_Pen.Color = Color;
				_Pen.Width = Width;
				_Pen.DashStyle = Style;
				return _Pen;
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
			vertex.OnPointMoved = OnPointMoved;

			if (Points.Count > 0)
			{
				var p1 = Points.Last(p => p is Vertex) as Vertex;
				var c1 = new ControlPoint(p1.ToPoint) { OnPointMoved = OnPointMoved, IsVisible = IsControlPointVisible };
				var c2 = new ControlPoint(vertex.ToPoint) { OnPointMoved = OnPointMoved, IsVisible = IsControlPointVisible };

				Points.Add(c1);
				Points.Add(c2);
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
			var vertex = adjacent.Find(p => p is Vertex);
			return !(vertex.X == ctrlPoint.X && vertex.Y == ctrlPoint.Y);
		}

		/// <summary>
		/// Called when the <paramref name="canvasPoint"/> is moved
		/// </summary>
		private void OnPointMoved(CanvasPoint canvasPoint, int dx, int dy, Keys modifierKeys)
		{
			if (canvasPoint is ControlPoint)
			{
				if (modifierKeys == Keys.Shift)
				{
					// Find the Vertex adjacent to ControlPoint
					var index = Points.IndexOf(canvasPoint);
					var vertexIndex = Points[index - 1] is Vertex ? index - 1 : index + 1;
					var vertex = Points.ElementAtOrDefault(vertexIndex);

					// Get the opposite control point
					var oppositeIndex = vertexIndex < index ? vertexIndex - 1 : vertexIndex + 1;
					var opposite = Points.ElementAtOrDefault(oppositeIndex);

					var offset = vertex.ToPoint.Subtract(canvasPoint.ToPoint);
					var newPoint = vertex.ToPoint.Add(offset);
					opposite?.MoveTo(newPoint, modifierKeys, true);
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
			orphanedCtrlPoint?.Let(p => Points.Remove(p));
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public override CanvasObject OnMouseDown(Point location, Keys modifierKeys, out bool allowMove)
		{
			activeObject = this;
			allowMove = true;

			if (IsSelected)
			{
				var vertex = Points.FirstOrDefault(p => p is Vertex && p.MouseOver(location, modifierKeys) != null);
				var ctrlPoint = Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, modifierKeys) != null);
				CanvasPoint point = vertex ?? ctrlPoint;

				// Check points
				if (point != null && modifierKeys == (Keys.Shift | Keys.Control))
				{
					// Delete point
					DeletePoint(point);
					allowMove = false;
				}
				else if (ctrlPoint != null && modifierKeys == Keys.Shift)
				{
					// Check ControlPoints points
					point = ctrlPoint;
				}

				if (point == null && modifierKeys == (Keys.Control | Keys.Shift))
				{
					TryAddVertex(location);
					allowMove = false;
				}

				activeObject = point ?? activeObject;
			}

			return activeObject;
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
				var vertex = new Vertex(location.X, location.Y) { OnPointMoved = OnPointMoved };
				var c1 = new ControlPoint(vertex.ToPoint) { OnPointMoved = OnPointMoved, IsVisible = IsControlPointVisible };
				var c2 = new ControlPoint(vertex.ToPoint) { OnPointMoved = OnPointMoved, IsVisible = IsControlPointVisible };
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
				ctrlPoint.MoveTo(canvasPoint.ToPoint, Keys.None);
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
		public override CanvasObject MouseOver(Point mousePosition, Keys modifierKeys)
		{
			CanvasObject objUnderMouse = null;

			if (Points.Count > 3)
			{
				// Check if over this line
				objUnderMouse = Path.IsOutlineVisible(mousePosition, new Pen(Color.Black, 10F)) ? this : null;

				if (objUnderMouse == null)
				{
					// Check if over any points that might be outside of the line
					objUnderMouse = Points.FirstOrDefault(p => p.MouseOver(mousePosition, modifierKeys) != null) != null ? this : null;
				}
			}

			return objUnderMouse;
		}

		/// <summary>
		/// Gets a <see cref="Cursor"/> representing the state of the <see cref="Line"/> that is represented
		/// by <paramref name="location"/>
		/// </summary>
		/// <returns><see cref="Cursor"/> represented by <paramref name="location"/></returns>
		public override Cursor GetCursor(Point location, Keys keys)
		{
			var cursor = Cursors.Hand;

			// Check if over any points
			if (IsSelected)
			{
				var vertex = Points.FirstOrDefault(p => p is Vertex && p.MouseOver(location, keys) != null);
				var ctrlPoint = Points.FirstOrDefault(p => p is ControlPoint && p.MouseOver(location, keys) != null);
				CanvasPoint point = vertex ?? ctrlPoint;

				if (point != null)
				{
					if (keys == (Keys.Control | Keys.Shift))
					{
						cursor = Resources.Cursors.RemovePoint;
					}
					else if (ctrlPoint != null && keys == Keys.Shift)
					{
						cursor = Resources.Cursors.AddCurve;
					}
					else
					{
						cursor = Resources.Cursors.MovePoint;
					}
				}
				else
				{
					if (keys == (Keys.Control | Keys.Shift))
					{
						cursor = Resources.Cursors.AddPoint;
					}
				}
			}

			return cursor;
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
		/// Moves <see cref="Line"/> by <paramref name="dx"/> and <paramref name="dy"/>
		/// </summary>
		public override void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false)
		{
			if (activeObject == this)
			{
				Points.FindAll(p => p is Vertex)?.ForEach(p => p.MoveBy(dx, dy, modifierKeys));
			}
			else
			{
				activeObject?.MoveBy(dx, dy, modifierKeys);
			}
		}

		/// <summary>
		/// Aligns <see cref="Line"/> to the <paramref name="alignInterval"/>
		/// </summary>
		public override void Align(int alignInterval) => Points.ForEach(p => p.Align(alignInterval));
	}
}