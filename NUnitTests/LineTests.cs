﻿using System.Diagnostics.CodeAnalysis;
using TNT.Drawing.Objects;

namespace NUnitTests;

[ExcludeFromCodeCoverage]
public class LineTests
{
  [Test]
  public void TestMethod1()
  {
    var line1 = new Line();
    var vertices = new List<Vertex>() { new Vertex(0, 0), new Vertex(10, 10), new Vertex(100, 100) };
    vertices.ForEach(v => line1.AddVertex(v));
    var line2 = line1.Copy() as Line;
    var line3 = line1;
    var line4 = new Line(line1);
  }
}
