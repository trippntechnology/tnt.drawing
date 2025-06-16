using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TNT.Commons;
using TNT.Drawing.Interface;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;
using TNT.Drawing.Resource;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Rotation <see cref="DrawingMode"/> for rotating objects on the canvas.
/// </summary>
/// <remarks>
/// Constructor for RotationMode
/// </remarks>
/// <param name="canvas">The canvas to interact with</param>
/// <param name="layer">The canvas layer containing objects</param>
public class RotationMode(Canvas canvas, CanvasLayer layer) : InteractionMode(canvas, layer)
{
  // Fields for rotation interaction
  private bool _isRotating = false;
  
  // Track current rotation angle for the icon
  private double _currentRotationAngle = 0;

  /// <summary>
  /// Handles mouse down event: starts rotation if a Vertex is under the mouse.
  /// </summary>
  public override void OnMouseDown(MouseEventArgs e, Keys modifierKeys)
  {
    var (hitObject, childObject, allowMove) = _objectUnderMouse?.OnMouseDown(e.Location, modifierKeys) ?? MouseDownResponse.Default;

    if (childObject == null)
    {
      if (_objectUnderMouse == null)
      {
        Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
      }
      else if (!_objectUnderMouse.IsSelected)
      {
        Layer.CanvasObjects.ForEach(o => o.IsSelected = false);
        _objectUnderMouse.IsSelected = true;
      }
    }
    else
    {
      _isRotating = true;
    }

    Layer.CanvasObjects.FindAll(o => o.IsSelected)?.Cast<object>().ToList()?.Also(o => Canvas.OnObjectsSelected(o));

    Canvas.Invalidate();

    // Update the feedback
    UpdateFeedback(e.Location, modifierKeys);

    base.OnMouseDown(e, modifierKeys);
  }
  /// <summary>
  /// Handles mouse movement: updates the object under the mouse and provides feedback.
  /// </summary>
  /// <param name="e">Mouse event arguments</param>
  /// <param name="modifierKeys">Current keyboard modifier keys</param>
  public override void OnMouseMove(MouseEventArgs e, Keys modifierKeys)
  {
    var location = e.Location;
    var rotationCentroid = _objectUnderMouse?.GetCentroid();

    if (_isRotating && _objectUnderMouse is IRotatable rotatable && rotationCentroid != null)
    {
      // Calculate angle from the rotation center to the current mouse position
      double currentAngle = Math.Atan2(location.Y - rotationCentroid.Value.Y, location.X - rotationCentroid.Value.X);

      // Calculate the angle from the rotation center to the previous mouse position
      double previousAngle = Math.Atan2(_lastMouseLocation.Y - rotationCentroid.Value.Y, _lastMouseLocation.X - rotationCentroid.Value.X);

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

    // The base class will handle updating the object under mouse and feedback
    base.OnMouseMove(e, modifierKeys);
  }

  /// <summary>
  /// Handles mouse up event: selects the object under the mouse if present.
  /// </summary>
  /// <param name="e">Mouse event arguments</param>
  /// <param name="modifierKeys">Current keyboard modifier keys</param>
  public override void OnMouseUp(MouseEventArgs e, Keys modifierKeys)
  {
    _isRotating = false;

    Canvas.Invalidate();
    base.OnMouseUp(e, modifierKeys);
  }

  public override void OnDraw(Graphics graphics)
  {
    base.OnDraw(graphics);

    Layer.CanvasObjects.FirstOrDefault(o => o.IsSelected)?.Also(selectedObject =>
    {
      // Draw the selected object
      selectedObject.Draw(graphics);

      // Calculate the center of mass (centroid) for the object
      Point? center = selectedObject.GetCentroid();

      if (center == null) return;

      // Get the rotation icon
      var rotateImage = Resources.Images.Rotate24;
      
      // Create a graphics state to apply the rotation transform
      GraphicsState state = graphics.Save();
      
      try
      {
        // Set up the transform for rotation
        graphics.TranslateTransform(center.Value.X, center.Value.Y);
        graphics.RotateTransform((float)_currentRotationAngle);
        
        // Draw the rotated image centered at the origin (which is now the centroid)
        graphics.DrawImage(rotateImage, 
            -rotateImage.Width / 2, 
            -rotateImage.Height / 2, 
            rotateImage.Width, 
            rotateImage.Height);
      }
      finally
      {
        // Restore the original graphics state
        graphics.Restore(state);
      }
    });
  }

  protected override void UpdateFeedback(Point location, Keys modifierKeys)
  {
    if (_objectUnderMouse is IRotatable)
    {
      base.UpdateFeedback(location, modifierKeys);
    }
    else { Canvas.OnFeedbackChanged(Feedback.Default); }
  }
}
