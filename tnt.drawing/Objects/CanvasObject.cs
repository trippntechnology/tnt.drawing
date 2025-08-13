using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TNT.Drawing.Model;
using TNT.Reactive;

namespace TNT.Drawing.Objects;

/// <summary>
/// Base class for all <see cref="Canvas"/> objects
/// </summary>
public abstract class CanvasObject : Observable
{
  protected const float OUTLINE_HIT_WIDTH = 10F;
  protected readonly Pen OUTLINE_PEN_HIT_WITDTH = new Pen(Color.Black, OUTLINE_HIT_WIDTH);

  /// <summary>
  /// <see cref="Guid"/> of the object
  /// </summary>
  [ReadOnly(true)]
  public string Id { get; set; }

  /// <summary>
  /// Indicates whether the object is selected or not
  /// </summary>
  public virtual bool IsSelected { get => Get(false); set => Set(value); }

  /// <summary>
  /// The rotation angle (in degrees) of the object.
  /// </summary>
  [DisplayName("Rotation Angle")]
  public double RotationAngle { get => Get(0.0); set => Set(value); }

  /// <summary>
  /// Default constructor
  /// </summary>
  protected CanvasObject() => Id = Guid.NewGuid().ToString();

  /// <summary>
  /// Copy constructor
  /// </summary>
  protected CanvasObject(CanvasObject canvasObject) => Id = canvasObject.Id;

  /// <summary>
  /// Implemented by subclass to draw object
  /// </summary>
  public abstract void Draw(Graphics graphics);

  /// <summary>
  /// Creates a deep copy of the current <see cref="CanvasObject"/> instance.
  /// Subclasses must implement this method to return a new instance with identical state.
  /// </summary>
  public abstract CanvasObject Clone();

  /// <summary>
  /// Implement to align the object at the given <paramref name="alignInterval"/>
  /// </summary>
  public abstract void Align(int alignInterval);

  /// <summary>
  /// Implement by subclass to indicate mouse is over object. Should return the object that is under mouse.
  /// </summary>
  public virtual MouseOverResponse MouseOver(Point mousePosition, Keys modifierKeys) => MouseOverResponse.Default;

  /// <summary>
  /// Determines whether this object intersects with the specified <see cref="Region"/>.
  /// Subclasses should override this method to provide intersection logic based on their geometry.
  /// Returns <c>false</c> by default.
  /// </summary>
  public virtual bool IntersectsWith(Region region) => false;

  /// <summary>
  /// Determines whether the mouse is currently positioned over this object.
  /// Subclasses should implement hit-testing logic based on their geometry and state.
  /// </summary>
  public abstract bool IsMouseOver(Point mousePosition, Keys modifierKeys);

  /// <summary>
  /// Implement by subclass to move the object by <paramref name="dx"/> and <paramref name="dy"/>
  /// </summary>
  public abstract void MoveBy(int dx, int dy, Keys modifierKeys, bool supressCallback = false);

  /// <summary>  
  /// Called when a button press event occurs over an object.  
  /// This method can be overridden to provide custom behavior when the mouse button is pressed.  
  /// </summary>  
  /// <param name="location">The location of the mouse pointer.</param>  
  /// <param name="modifierKeys">The modifier keys pressed during the event.</param>  
  /// <returns>A <see cref="MouseDownResponse"/> object indicating the result of the mouse down event.</returns>  
  public virtual MouseDownResponse OnMouseDown(Point location, Keys modifierKeys) => MouseDownResponse.Default;

  /// <summary>
  /// Called when a mouse button is released over an object.
  /// This method can be overridden to provide custom behavior when the mouse button is released.
  /// </summary>
  /// <param name="location">The location of the mouse pointer.</param>
  /// <param name="modifierKeys">The modifier keys pressed during the event.</param>
  /// <returns>A <see cref="MouseUpResponse"/> object indicating the result of the mouse up event.</returns>
  public virtual MouseUpResponse OnMouseUp(Point location, Keys modifierKeys) => MouseUpResponse.Default;

  /// <summary>
  /// Implement to return a <see cref="Feedback"/> indicating the <see cref="Cursor"/> and hint
  /// to display
  /// </summary>
  /// <returns><see cref="Feedback"/> indicating the <see cref="Cursor"/> and hint to display</returns>
  public virtual Feedback GetFeedback(Point location, Keys modifierKeys) => Feedback.Default;

  /// <summary>
  /// Returns the centroid (center of mass) of the object in canvas coordinates, if defined.
  /// Subclasses should override this method to provide the centroid for their specific geometry.
  /// Returns null if the centroid is not defined for this object.
  /// </summary>
  public virtual Point? GetCentroidPosition() => null;

  /// <summary>
  /// Determines whether the specified object is equal to the current <see cref="CanvasObject"/>.
  /// Two <see cref="CanvasObject"/> instances are considered equal if their <see cref="Id"/> properties are equal.
  /// </summary>
  /// <param name="obj">The object to compare with the current object.</param>
  /// <returns><c>true</c> if the specified object is a <see cref="CanvasObject"/> with the same <see cref="Id"/>; otherwise, <c>false</c>.</returns>
  public override bool Equals(object? obj) => obj is CanvasObject canvasObject && Id == canvasObject.Id;

  /// <summary>
  /// Returns a hash code for this <see cref="CanvasObject"/> based on its <see cref="Id"/> property.
  /// </summary>
  /// <returns>A hash code for the current object.</returns>
  public override int GetHashCode() => Id.GetHashCode();
}
