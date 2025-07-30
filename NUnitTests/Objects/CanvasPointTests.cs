using System.Diagnostics.CodeAnalysis;
using TNT.Drawing.Objects;

namespace NUnitTests.Objects;

[ExcludeFromCodeCoverage]
public class CanvasPointTests
{
  [Test]
  public void IsNear_ReturnsTrue_WhenPointsAreIdentical()
  {
    var p1 = new CanvasPoint(10, 20);
    var p2 = new CanvasPoint(10, 20);
    Assert.That(p1.IsNear(p2), Is.True, "IsNear should return true for identical points.");
  }

  [Test]
  public void IsNear_ReturnsTrue_WhenPointsWithinDefaultThreshold()
  {
    var p1 = new CanvasPoint(0, 0);
    var p2 = new CanvasPoint(5, 5); // sqrt(25+25)=~7.07 < 8
    Assert.That(p1.IsNear(p2), Is.True, "IsNear should return true when points are within default threshold.");
  }

  [Test]
  public void IsNear_ReturnsFalse_WhenPointsOutsideDefaultThreshold()
  {
    var p1 = new CanvasPoint(0, 0);
    var p2 = new CanvasPoint(11, 0); // distance = 11 > 10
    Assert.That(p1.IsNear(p2), Is.False, "IsNear should return false when points are outside default threshold.");
  }

  [Test]
  public void IsNear_UsesCustomThreshold()
  {
    var p1 = new CanvasPoint(0, 0);
    var p2 = new CanvasPoint(15, 0);
    Assert.That(p1.IsNear(p2, threshold: 15), Is.True, "IsNear should return true when within custom threshold.");
    Assert.That(p1.IsNear(p2, threshold: 10), Is.False, "IsNear should return false when outside custom threshold.");
  }

  [Test]
  public void IsNear_ReturnsTrue_WhenDistanceEqualsThreshold()
  {
    var p1 = new CanvasPoint(0, 0);
    var p2 = new CanvasPoint(8, 0); // distance = 8 == default threshold
    Assert.That(p1.IsNear(p2), Is.True, "IsNear should return true when distance equals threshold.");
  }
}
