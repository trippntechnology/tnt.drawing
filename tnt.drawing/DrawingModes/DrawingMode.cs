﻿using System.Collections.Generic;
﻿using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;

namespace TNT.Drawing.DrawingModes;

/// <summary>
/// Base class for all drawing modes
/// </summary>
public class DrawingMode(Canvas canvas, CanvasLayer layer, CanvasObject? defaultObject = null)
{
  /// <summary>
  /// Indicates when the mouse button is pressed
  /// </summary>
  protected bool IsMouseDown = false;

  /// <summary>
  /// The active vertex that is being manipulated in the drawing mode.
  /// </summary>
  protected Vertex activeVertex = new Vertex();

  /// <summary>
  /// List of vertices that are being drawn in the current drawing mode.
  /// </summary>
  protected List<Vertex> vertices = new List<Vertex>();

  /// <summary>
  /// Reference to the <see cref="Canvas"/>
  /// </summary>
  protected Canvas Canvas { get; private set; } = canvas;

  /// <summary>
  /// The <see cref="CanvasLayer"/> manipulated by this <see cref="DrawingMode"/>
  /// </summary>
  public CanvasLayer Layer { get; private set; } = layer;

  /// <summary>
  /// The <see cref="CanvasObject"/> that gets created by the <see cref="DrawingMode"/>
  /// </summary>
  public CanvasObject? DefaultObject { get; } = defaultObject;

  /// <summary>
  /// Refreshes the <see cref="Canvas"/> with the current <see cref="CanvasLayer"/>
  /// </summary>
  public virtual void Reset()
  {
    vertices.Clear();
    Canvas.Invalidate();
  }

  /// <summary>
  /// Called when the mouse moves across the <see cref="Canvas"/>
  /// </summary>
  public virtual void OnMouseMove(MouseEventArgs e, Keys modifierKeys) => Log();

  /// <summary>
  /// Called when the mouse button is pressed
  /// </summary>
  public virtual void OnMouseDown(MouseEventArgs e, Keys modifierKeys) { IsMouseDown = true; Log(); }

  /// <summary>
  /// Called when the mouse button is released
  /// </summary>
  public virtual void OnMouseUp(MouseEventArgs e, Keys modifierKeys) { IsMouseDown = false; Log(); }

  /// <summary>
  /// Called when the mouse button is double clicked
  /// </summary>
  public virtual void OnMouseDoubleClick(MouseEventArgs e) => Log();

  /// <summary>
  /// Called when a key is pressed
  /// </summary>
  public virtual void OnKeyDown(KeyEventArgs e) => Log($"{e.KeyCode}");

  /// <summary>
  /// Called when a key is released
  /// </summary>
  public virtual void OnKeyUp(KeyEventArgs e) => Log($"{e.KeyCode}");

  /// <summary>
  /// Called when the <see cref="Canvas"/> is being painted
  /// </summary>
  public virtual void OnDraw(Graphics graphics) => Log();

  /// <summary>
  /// Called to log to <see cref="Debug"/>
  /// </summary>
  protected virtual void Log(string msg = "", [CallerMemberName] string callingMethod = "") { }// => TNTLogger.Info(msg, callingMethod);
}
