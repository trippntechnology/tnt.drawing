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
    public static Image PolylineTool { get; } = ResourceToImage("TNT.Drawing.Resource.Image.polyline_24dp.png");
    public static Image RectangleTool { get; } = ResourceToImage("TNT.Drawing.Resource.Image.rectangle_24dp.png");
    public static Image Rotate { get; } = ResourceToImage("TNT.Drawing.Resource.Image.directory_sync_24dp.png");
    public static Image SelectorTool { get; } = ResourceToImage("TNT.Drawing.Resource.Image.arrow_selector_tool_24dp.png");
  }

  /// <summary>
  /// Contains embedded cursor resources for custom drawing operations.
  /// </summary>
  public static class Cursors
  {
    public static Cursor Add { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Add.cur");
    public static Cursor AddControlPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddControlPoint.cur");
    public static Cursor AddSelection { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddSelection.cur");
    public static Cursor Drag { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Drag.cur");
    public static Cursor DragControlPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DragControlPoint.cur");
    public static Cursor DragPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DragPoint.cur");
    public static Cursor DrawClosedPolyLine { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DrawClosedPolyline.cur");
    public static Cursor DrawPolyLine { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DrawPolyline.cur");
    public static Cursor DrawRectangle { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.DrawRectangle.cur");
    public static Cursor Hand { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Hand.cur");
    public static Cursor HideControlPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.HideCtrlPoint.cur");
    public static Cursor Remove { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Remove.cur");
    public static Cursor RemoveSelection { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.RemoveSelection.cur");
    public static Cursor Rotate { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Rotate.cur");
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
