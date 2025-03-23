using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using TNT.Commons;
using TNT.Drawing;

namespace NUnitTests;

[ExcludeFromCodeCoverage]
public class ExtensionsTests
{
  [Test]
  public void PointSubstract()
  {
    var p1 = new Point(11, 7);
    var p2 = new Point(31, 29);
    var expected = new Point(20, 22);
    Assert.That(expected, Is.EqualTo(p2.Subtract(p1)));
  }

  [Test]
  public void PointAdd()
  {
    var p1 = new Point(11, 7);
    var p2 = new Point(31, 29);
    var expected = new Point(42, 36);
    Assert.That(expected, Is.EqualTo(p1.Add(p2)));
  }

  [Test]
  public void Snap()
  {
    Assert.That(new Point(11, 19).Snap(10), Is.EqualTo(new Point(10, 20)));
    Assert.That(new Point(14, 16).Snap(10), Is.EqualTo(new Point(10, 20)));
    Assert.That(new Point(15, 15).Snap(10), Is.EqualTo(new Point(20, 20)));
    Assert.That(new Point(16, 14).Snap(10), Is.EqualTo(new Point(20, 10)));
    Assert.That(new Point(19, 11).Snap(10), Is.EqualTo(new Point(20, 10)));
    Assert.That(new Point(20, 20).Snap(10), Is.EqualTo(new Point(20, 20)));
  }

  [Test]
  public void Deconstruct()
  {
    var p1 = new Point(11, 7);
    Assert.That(p1.Deconstruct(), Is.EqualTo((11, 7)));
  }


  [Test]
  public void AddNotNull()
  {
    var list = new List<int?>();
    int? v1 = null;
    list.AddNotNull(v1);
    Assert.That(list.Count, Is.EqualTo(0));
    v1 = 1;
    list.AddNotNull(v1);
    Assert.That(list.Count, Is.EqualTo(1));
  }

  [Test]
  public void AdjacentTo()
  {
    var sut = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    var result = sut.AdjacentTo(5);
    Assert.That(result, Is.EqualTo(new List<int>() { 4, 6 }).AsCollection);
  }
}
