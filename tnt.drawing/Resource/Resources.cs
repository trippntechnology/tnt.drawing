using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TNT.Drawing.Resource
{
	public static class Resources
	{
		private static BackingFields backingFields = new BackingFields();

		public static class Images
		{
			public static Image ControlPoint { get; } = ResourceToImage("TNT.Drawing.Resource.Image.ControlPoint.png");
			public static Image Vertex { get; } = ResourceToImage("TNT.Drawing.Resource.Image.Vertex.png");
		}
		public static class Cursors
		{
			public static Cursor AddCurve { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddCurve.cur");
			public static Cursor AddPoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.AddPoint.cur");
			public static Cursor Curve { get;} = ResourceToCursor("TNT.Drawing.Resource.Cursor.Curve.cur");
			public static Cursor MovePoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.MovePoint.cur");
			public static Cursor RemovePoint { get; } = ResourceToCursor("TNT.Drawing.Resource.Cursor.RemovePoint.cur");
		}

		/// <summary>
		/// Gets an image associated with the <paramref name="resource"/> value within the calling assembly
		/// </summary>
		public static Image ResourceToImage(string resource)
		{
			var assembly = Assembly.GetCallingAssembly();
			var resourceStream = assembly.GetManifestResourceStream(resource);
			return resourceStream == null ? null : new Bitmap(resourceStream);
		}

		public static Cursor ResourceToCursor(string resource) => Utilities.Utilities.LoadColorCursor(resource);
	}
}
