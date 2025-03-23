using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TNT.Drawing.Resource
{
  /// <summary>
  /// Resources
  /// </summary>
  public static class Resources
  {
    private static BackingFields backingFields = new BackingFields();

    /// <summary>
    /// Images
    /// </summary>
    public static class Images
    {
      /// <summary>
      /// <see cref="ControlPoint"/> <see cref="Image"/>
      /// </summary>
      public static Image ControlPoint { get; } = ResourceToImage("TNT.Drawing.Resource.Image.ControlPoint.png");

      /// <summary>
      /// <see cref="Vertex"/> <see cref="Image"/>
      /// </summary>
      public static Image Vertex { get; } = ResourceToImage("TNT.Drawing.Resource.Image.Vertex.png");
    }

    /// <summary>
    /// Cursors
    /// </summary>
    public static class Cursors
    {
      /// <summary>
      /// <see cref="Cursor"/> that represents an Add Curve action
      /// </summary>
      public static Cursor AddCurve { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddCurve.cur");

      /// <summary>
      /// <see cref="Cursor"/> that represents an Add Point action
      /// </summary>
      public static Cursor AddPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddPoint.cur");

      /// <summary>
      /// <see cref="Cursor"/> that represents an Add Curve action
      /// </summary>
      public static Cursor Curve { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.Curve.cur");

      /// <summary>
      /// <see cref="Cursor"/> that represents an Move Point action
      /// </summary>
      public static Cursor MovePoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.MovePoint.cur");

      /// <summary>
      /// <see cref="Cursor"/> that represents an Remove Point action
      /// </summary>
      public static Cursor RemovePoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.RemovePoint.cur");
    }

    /// <summary>
    /// Gets an image associated with the <paramref name="resource"/> value within the calling assembly
    /// </summary>
    public static Image ResourceToImage(string resource)
    {
      var assembly = Assembly.GetCallingAssembly();
      var resourceStream = assembly.GetManifestResourceStream(resource);
      var image = resourceStream == null ? null : new Bitmap(resourceStream);
      if (image == null) throw new Exception($"Resource {resource} not found");
      return image;
    }

    /// <summary>
    /// Converts <paramref name="resource"/> to <see cref="Cursor"/>
    /// </summary>
    /// <returns><see cref="Cursor"/> represented by the <paramref name="resource"/></returns>
    public static Cursor ResourceToCursor(string resource)
    {
      var cursor = Utilities.Utilities.LoadColorCursor(resource);
      if (cursor == null) throw new Exception($"Resource {resource} not found");
      return cursor;
    }
  }
}
