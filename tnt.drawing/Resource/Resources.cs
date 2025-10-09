using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TNT.Drawing.Resource;

/// <summary>
/// Provides access to embedded image and cursor resources for TNT.Drawing.
/// Resources are loaded from the assembly manifest and exposed as static properties.
/// </summary>
public static class Resources
{
  /// <summary>
  /// Contains embedded image resources used for drawing controls and icons.
  /// </summary>
  public static class Images
  {
    /// <summary>
    /// Control point icon image resource.
    /// </summary>
    public static Image ControlPoint { get; } = ResourceToImage("TNT.Drawing.Resource.Image.ControlPoint.png");

    /// <summary>
    /// Vertex icon image resource.
    /// </summary>
    public static Image Vertex { get; } = ResourceToImage("TNT.Drawing.Resource.Image.Vertex.png");

    /// <summary>
    /// Extra small rotate icon image (16x16).
    /// </summary>
    public static Image Rotate16 { get; } = ResourceToImage("TNT.Drawing.Resource.Image.Rotate_16.png");

    /// <summary>
    /// Small rotate icon image (24x24).
    /// </summary>
    public static Image Rotate24 { get; } = ResourceToImage("TNT.Drawing.Resource.Image.Rotate_24.png");

    /// <summary>
    /// Large rotate icon image (48x48).
    /// </summary>
    public static Image Rotate48 { get; } = ResourceToImage("TNT.Drawing.Resource.Image.Rotate_48.png");
  }

  /// <summary>
  /// Contains embedded cursor resources for custom drawing operations.
  /// </summary>
  public static class Cursors
  {
    /// <summary>
    /// Cursor for Add Curve action.
    /// </summary>
    public static Cursor AddCurve { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddCurve.cur");

    /// <summary>
    /// Cursor for Add Point action.
    /// </summary>
    public static Cursor AddPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddPoint.cur");

    /// <summary>
    /// Cursor for Curve action.
    /// </summary>
    public static Cursor Curve { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Curve.cur");

    /// <summary>
    /// Cursor for Move Point action.
    /// </summary>
    public static Cursor MovePoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.MovePoint.cur");

    /// <summary>
    /// Cursor for Remove Point action.
    /// </summary>
    public static Cursor RemovePoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.RemovePoint.cur");

    /// <summary>
    /// Cursor for Rotate action.
    /// </summary>
    public static Cursor Rotate { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Rotate.cur");

    /// <summary>
    /// Cursor for Line selection.
    /// </summary>
    public static Cursor DrawLine { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DrawLine.cur");

    /// <summary>
    /// Cursor for rectangle selection (Aero style).
    /// </summary>
    public static Cursor DrawRectangle { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DrawRectangle.cur");

    public static Cursor Hand { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Hand.cur");
    public static Cursor Move { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Move.cur");
    public static Cursor Add { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Add.cur");
    public static Cursor Remove { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Remove.cur");
    public static Cursor AddControlPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddControlPoint.cur");
    public static Cursor Drag { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Drag.cur");

    public static Cursor DragControlPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DragControlPoint.cur");
    public static Cursor DragPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DragPoint.cur");
    public static Cursor HideControlPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.HideCtrlPoint.cur");

    public static Cursor AddSelection { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddSelection.cur");
    public static Cursor RemoveSelection { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.RemoveSelection.cur");


  }

  /// <summary>
  /// Loads an embedded image resource from the calling assembly.
  /// Throws an exception if the resource is not found.
  /// </summary>
  /// <param name="resource">Resource name (manifest path)</param>
  /// <returns>Image loaded from resource</returns>
  /// <exception cref="Exception">Thrown if resource is not found</exception>
  private static Image ResourceToImage(string resource)
  {
    var assembly = Assembly.GetCallingAssembly();
    var resourceStream = assembly.GetManifestResourceStream(resource);
    var image = resourceStream == null ? null : new Bitmap(resourceStream);
    if (image == null) throw new Exception($"Resource {resource} not found");
    return image;
  }

  /// <summary>
  /// Loads an embedded cursor resource from the calling assembly.
  /// Returns the default cursor if the resource is not found.
  /// </summary>
  /// <param name="resourceName">Resource name (manifest path)</param>
  /// <returns>Cursor loaded from resource or default cursor</returns>
  private static Cursor ResourceToCursor(string resourceName)
  {
    // Get the name of a temporary file
    string path = Path.GetTempFileName();

    // Put the resource into a stream and save it to a file
    using Stream? stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName);

    if (stream == null) return System.Windows.Forms.Cursors.Default;

    using (FileStream fileStream = File.Create(path, (int)stream.Length))
    {
      // Fill the bytes[] array with the stream data         
      byte[] bytesInStream = new byte[stream.Length];
      stream.ReadExactly(bytesInStream, 0, bytesInStream.Length);

      // Use FileStream object to write to the specified file         
      fileStream.Write(bytesInStream, 0, bytesInStream.Length);
    }

    // Load the cursor from the file and delete file
    Cursor cursor = new Cursor(LoadCursorFromFile(path));
    File.Delete(path);

    return cursor;
  }

  /// <summary>
  /// Loads a cursor from a file using Win32 API.
  /// </summary>
  /// <param name="lpFileName">Path to cursor file</param>
  /// <returns>Pointer to loaded cursor</returns>
  [DllImport("user32.dll", CharSet = CharSet.Unicode)]
  private static extern IntPtr LoadCursorFromFile(string lpFileName);

}
