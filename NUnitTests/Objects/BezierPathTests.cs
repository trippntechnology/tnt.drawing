using System.Diagnostics.CodeAnalysis;
using TNT.Drawing.Objects;

namespace NUnitTests.Objects;

[ExcludeFromCodeCoverage]
public class BezierPathTests
{
  [Test]
  public void TestMethod1()
  {
    var line1 = new BezierPath();
    var vertices = new List<Vertex>() { new Vertex(0, 0), new Vertex(10, 10), new Vertex(100, 100) };
    vertices.ForEach(v => line1.AddVertex(v));
    var line2 = line1.Clone() as BezierPath;
    var line3 = line1;
    var line4 = new BezierPath(line1);
  }
}
