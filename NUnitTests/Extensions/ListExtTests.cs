using System.Diagnostics.CodeAnalysis;
using TNT.Drawing.Extentions;

namespace NUnitTests.Extensions;

[ExcludeFromCodeCoverage]
public class ListExtTests
{
  [Test]
  public void AdjacentTo()
  {
    var sut = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    var result = sut.AdjacentTo(5);
    Assert.That(result, Is.EqualTo(new List<int>() { 4, 6 }).AsCollection);
  }
}
