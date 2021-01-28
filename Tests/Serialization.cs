using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TNT.Drawing.Layers;
using TNT.Drawing.Objects;
using TNT.Utilities;

namespace Tests
{
	[TestClass]
	public class Serialization
	{
		[TestMethod]
		public void Line()
		{
			var path =Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location);
			var assPath = Path.Combine(path, "tnt.drawing.dll");
			var types = Utilities.GetTypes(assPath, t => !t.IsAbstract && (t.InheritsFrom(typeof(CanvasObject)) || t.InheritsFrom(typeof(CanvasLayer))));

			var line = new Line();
			line.AddVertex(new Vertex(10, 10));
			line.AddVertex(new Vertex(100, 100));

			var text = Utilities.Serialize(line, types);

			var newLine = Utilities.Deserialize<Line>(text, types);
		}
	}
}
