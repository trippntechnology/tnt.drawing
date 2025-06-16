using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Extensions;
using TNT.Drawing.Interface;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;
using TNT.Drawing.Resource;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Select <see cref="DrawingMode"/> for selecting and moving objects on the canvas.
/// </summary>
/// <remarks>
/// Constructor for SelectMode
/// </remarks>
/// <param name="canvas">The canvas to interact with</param>
/// <param name="layer">The canvas layer containing objects</param>
public class SelectMode(Canvas canvas, CanvasLayer layer) : InteractionMode(canvas, layer)
{
  private bool _allowMove = false;
  private bool _isRotatingObject = false;
  private Rectangle _rotationHandleRect = Rectangle.Empty;
  private Point? _rotationCenter = null;

  // Track current rotation angle for the icon
  private double _currentRotationAngle = 0;

  // Constants for rotation handle appearance
  private const int CentroidDotSize = 4; // Size of dot at centroid point
  private const int RotationHandleHitArea = 20; // Size of the hit area for the rotation handle

  /// <summary>
  /// Determines if a point is over the rotation handle
  /// </summary>
  /// <param name="point">The point to check</param>
  /// <returns>True if the point is over the rotation handle, false otherwise</returns>
  private bool IsPointOverRotationHandle(Point point)
  {
    return _rotationHandleRect.Contains(point);
  }

  /// <summary>
  /// Handles mouse down event for selection
  /// </summary>
  public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
  {
    // Get the current selected objects before anything changes
    var previouslySelectedObjects = Layer.CanvasObjects.FindAll(o => o.IsSelected);
    
    // Handling rotation takes precedence over other operations
    if (IsPointOverRotationHandle(e.Location))
    {
      _isRotatingObject = true;
      _allowMove = false;

      // Store rotation center point for calculations
      var selectedObjects = Layer.CanvasObjects.FindAll(o => o.IsSelected);
      if (selectedObjects.Count == 1)
      {
        _rotationCenter = selectedObjects[0].GetCentroid();
      }

      // Important: Do not return early, allow the base class to handle mouse down
    }
    else
    {
      // Normal selection handling
      var (hitObject, childObject, allowMove) = _objectUnderMouse?.OnMouseDown(e.Location, modifierKeys) ?? MouseDownResponse.Default;

      _allowMove = allowMove;
      _isRotatingObject = false;

      if (childObject == null)
      {
        if (_objectUnderMouse == null)
        {
          Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
        }
        else if (modifierKeys == Keys.Control)
        {
          _objectUnderMouse.IsSelected = !_objectUnderMouse.IsSelected;
        }
        else if (!_objectUnderMouse.IsSelected)
        {
          Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
          _objectUnderMouse.IsSelected = true;
        }
      }

      // Get the newly selected objects
      var nowSelectedObjects = Layer.CanvasObjects.FindAll(o => o.IsSelected);
      
      // Reset rotation angle if the selection has changed - either different objects
      // or different count of objects
      bool selectionChanged = 
          previouslySelectedObjects.Count != nowSelectedObjects.Count || 
          !previouslySelectedObjects.All(o => nowSelectedObjects.Contains(o));
      
      if (selectionChanged)
      {
        _currentRotationAngle = 0;
        // Reset the rotation handle rectangle when selection changes
        _rotationHandleRect = Rectangle.Empty;
      }

      Layer.CanvasObjects.FindAll(o => o.IsSelected)?.Cast<object>().ToList()?.Also(o => Canvas.OnObjectsSelected(o));
    }

    Canvas.Invalidate();

    // Update the feedback
    UpdateFeedback(e.Location, modifierKeys);

    // Always call base.OnMouseDown to ensure proper state tracking
    base.OnMouseDown(e, modifierKeys);
  }

  /// <summary>
  /// Handles mouse movement during selection mode. When the mouse is down and movement is allowed,
  /// it moves selected objects by the distance the mouse has moved.
  /// </summary>
  /// <param name="e">Contains mouse position and button information</param>
  /// <param name="modifierKeys">Current keyboard modifier keys being pressed</param>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    var location = Canvas.SnapToInterval == modifierKeys.DoesNotContain(Keys.Control) ? e.Location.Snap(Canvas.SnapInterval) : e.Location;

    // Handle rotation if we're in rotation mode
    if (IsMouseDown && _isRotatingObject && _rotationCenter.HasValue)
    {
      var selectedObject = Layer.CanvasObjects.FindAll(o => o.IsSelected).FirstOrDefault();

      if (selectedObject is IRotatable rotatable)
      {
        Point center = _rotationCenter.Value;

        // Calculate angle from the rotation center to the current mouse position
        double currentAngle = Math.Atan2(location.Y - center.Y, location.X - center.X);

        // Calculate the angle from the rotation center to the previous mouse position
        double previousAngle = Math.Atan2(_lastMouseLocation.Y - center.Y, _lastMouseLocation.X - center.X);

        // Calculate the change in angle (delta)
        double deltaAngle = currentAngle - previousAngle;

        // Convert radians to degrees
        double deltaDegrees = deltaAngle * 180.0 / Math.PI;

        // Apply rotation to the object if there's a significant change
        if (Math.Abs(deltaDegrees) > 0.1)
        {
          rotatable.RotateBy(deltaDegrees, modifierKeys);

          // Update the current rotation angle for the icon
          _currentRotationAngle += deltaDegrees;

          Canvas.Invalidate();
        }
      }
    }
    // Regular object movement
    else if (IsMouseDown && _allowMove)
    {
      var dx = location.X - _lastMouseLocation.X;
      var dy = location.Y - _lastMouseLocation.Y;

      Layer.CanvasObjects.FindAll(o => o.IsSelected).ForEach(o => o.MoveBy(dx, dy, modifierKeys));
      Canvas.Invalidate();
    }

    base.OnMouseMove(e, modifierKeys);
  }

  /// <summary>
  /// Handles mouse up events, ending any ongoing operations
  /// </summary>
  /// <param name="e">Contains mouse button information</param>
  /// <param name="modifierKeys">Current keyboard modifier keys being pressed</param>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
  {
    _isRotatingObject = false;
    _rotationCenter = null;

    Canvas.Invalidate();
    base.OnMouseUp(e, modifierKeys);
  }

  /// <summary>
  /// Draws the selected objects
  /// </summary>
  public override void OnDraw(Graphics graphics)
  {
    // Draw all selected objects
    var selectedObjects = Layer.CanvasObjects.FindAll(o => o.IsSelected);
    selectedObjects.ForEach(o => o.Draw(graphics));

    // Only show rotation handle when exactly one object is selected
    if (selectedObjects.Count == 1)
    {
      var selectedObject = selectedObjects[0];

      // Get the centroid of the object (center point)
      Point? center = selectedObject.GetCentroid();

      if (center == null) return;

      // Get the rotation icon
      var rotateImage = Resources.Images.Rotate16;

      // Update the rotation handle hit rectangle for hit testing
      int hitAreaSize = Math.Max(rotateImage.Width, RotationHandleHitArea);
      _rotationHandleRect = new Rectangle(
          center.Value.X - hitAreaSize / 2,
          center.Value.Y - hitAreaSize / 2,
          hitAreaSize,
          hitAreaSize
      );

      // Create a graphics state to apply the rotation transform
      GraphicsState state = graphics.Save();

      try
      {
        // Set up the transform for rotation
        graphics.TranslateTransform(center.Value.X, center.Value.Y);
        graphics.RotateTransform((float)_currentRotationAngle);

        // Draw background circle for the handle (for better visibility)
        using (SolidBrush circleBrush = new SolidBrush(_isRotatingObject ? Color.FromArgb(220, Color.LightBlue) : Color.FromArgb(220, Color.White)))
        {
          int circleSize = rotateImage.Width + 6; // Larger background circle
          graphics.FillEllipse(circleBrush, -circleSize / 2, -circleSize / 2, circleSize, circleSize);
        }

        // Draw the rotated image centered at the origin (which is now the centroid)
        graphics.DrawImage(rotateImage, -rotateImage.Width / 2, -rotateImage.Height / 2, rotateImage.Width, rotateImage.Height);
      }
      finally
      {
        // Restore the original graphics state
        graphics.Restore(state);
      }
    }
    else
    {
      // Ensure rotation handle rect is reset when no object or multiple objects are selected
      _rotationHandleRect = Rectangle.Empty;
    }

    base.OnDraw(graphics);
  }

  /// <summary>
  /// Updates feedback cursor and tooltip based on what's under the mouse
  /// </summary>
  protected override void UpdateFeedback(Point location, Keys modifierKeys)
  {
    // Change cursor to rotation cursor when over the rotation handle
    if (IsPointOverRotationHandle(location))
    {
      Canvas.OnFeedbackChanged(new Feedback(Cursors.Hand, "Click and drag to rotate object"));
      return;
    }

    // Default behavior for other areas
    base.UpdateFeedback(location, modifierKeys);
  }
}
