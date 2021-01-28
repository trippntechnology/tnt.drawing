using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TNT.Drawing.Objects;

namespace Tests
{
	[TestClass]
	public class LineTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			var line1 = new Line();
			var vertices = new List<Vertex>() { new Vertex(0, 0), new Vertex(10, 10), new Vertex(100, 100) };
			vertices.ForEach(v => line1.AddVertex(v));
			var line2 = line1.Copy() as Line;
			var line3 = line1;
			var line4 = new Line(line1);

			var i = 0;

		}
	}
}
