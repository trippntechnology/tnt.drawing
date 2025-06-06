using System.Diagnostics.CodeAnalysis;
using TNT.Drawing.Extensions;
using TNT.Drawing.Objects;

namespace NUnitTests.Extensions;

/// <summary>
/// Unit tests for ListExt extension methods.
/// </summary>
[ExcludeFromCodeCoverage]
public class ListExtTests
{
  [Test]
  public void IsFirst_ReturnsTrue_WhenItemIsFirst()
  {
    var list = new List<int> { 10, 20, 30 };
    Assert.That(list.IsFirst(10), Is.True);
  }

  [Test]
  public void IsFirst_ReturnsFalse_WhenItemIsNotFirst()
  {
    var list = new List<int> { 10, 20, 30 };
    Assert.That(list.IsFirst(20), Is.False);
    Assert.That(list.IsFirst(30), Is.False);
  }

  [Test]
  public void IsFirst_ReturnsFalse_WhenListIsEmpty()
  {
    var list = new List<int>();
    Assert.That(list.IsFirst(1), Is.False);
  }

  [Test]
  public void IsFirst_ReturnsFalse_WhenItemNotInList()
  {
    var list = new List<int> { 1, 2, 3 };
    Assert.That(list.IsFirst(99), Is.False);
  }

  [Test]
  public void IsLast_ReturnsTrue_WhenItemIsLast()
  {
    var list = new List<int> { 10, 20, 30 };
    Assert.That(list.IsLast(30), Is.True);
  }

  [Test]
  public void IsLast_ReturnsFalse_WhenItemIsNotLast()
  {
    var list = new List<int> { 10, 20, 30 };
    Assert.That(list.IsLast(10), Is.False);
    Assert.That(list.IsLast(20), Is.False);
  }

  [Test]
  public void IsLast_ReturnsFalse_WhenListIsEmpty()
  {
    var list = new List<int>();
    Assert.That(list.IsLast(1), Is.False);
  }

  [Test]
  public void IsLast_ReturnsFalse_WhenItemNotInList()
  {
    var list = new List<int> { 1, 2, 3 };
    Assert.That(list.IsLast(99), Is.False);
  }

  [Test]
  public void FindCoincident_ReturnsNull_WhenPointIsNotFirstOrLast()
  {
    var points = new List<CanvasPoint> {
            new CanvasPoint(0, 0),
            new CanvasPoint(10, 0),
            new CanvasPoint(20, 0)
        };
    var result = points.FindCoincident(points[1]);
    Assert.That(result, Is.Null);
  }

  [Test]
  public void FindCoincident_ReturnsNull_WhenNoCoincidenceAtEnds()
  {
    var points = new List<CanvasPoint> {
            new CanvasPoint(0, 0),
            new CanvasPoint(10, 0),
            new CanvasPoint(100, 0)
        };
    var resultFirst = points.FindCoincident(points[0], threshold: 5);
    var resultLast = points.FindCoincident(points[2], threshold: 5);
    Assert.That(resultFirst, Is.Null);
    Assert.That(resultLast, Is.Null);
  }

  [Test]
  public void FindCoincident_ReturnsOppositeEnd_WhenCoincidentWithinThreshold()
  {
    var points = new List<CanvasPoint> {
            new CanvasPoint(0, 0),
            new CanvasPoint(10, 0),
            new CanvasPoint(1, 1)
        };
    // points[0] and points[2] are near each other
    var resultFirst = points.FindCoincident(points[0], threshold: 2);
    var resultLast = points.FindCoincident(points[2], threshold: 2);
    Assert.That(resultFirst, Is.EqualTo(points[2]));
    Assert.That(resultLast, Is.EqualTo(points[0]));
  }

  [Test]
  public void FindCoincident_ReturnsNull_WhenListIsEmpty()
  {
    var points = new List<CanvasPoint>();
    var point = new CanvasPoint(0, 0);
    var result = points.FindCoincident(point);
    Assert.That(result, Is.Null);
  }

  [Test]
  public void FindCoincident_ReturnsNull_WhenPointNotInList()
  {
    var points = new List<CanvasPoint> {
            new CanvasPoint(0, 0),
            new CanvasPoint(10, 0)
        };
    var point = new CanvasPoint(100, 100);
    var result = points.FindCoincident(point);
    Assert.That(result, Is.Null);
  }

  [Test]
  public void AdjacentTo()
  {
    var sut = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    var result = sut.AdjacentTo(5);
    Assert.That(result, Is.EqualTo(new List<int>() { 4, 6 }).AsCollection);
  }

  [Test]
  public void AddNotNull_AddsValue_WhenValueIsNotNull()
  {
    // Arrange
    var list = new List<string>();
    var value = "Test";

    // Act
    list.AddNotNull(value);

    // Assert
    Assert.That(list, Does.Contain(value));
  }

  [Test]
  public void AddNotNull_DoesNotAddValue_WhenValueIsNull()
  {
    // Arrange
    var list = new List<string>();
    string? value = null;

    // Act
    list.AddNotNull(value);

    // Assert
    Assert.That(list, Is.Empty);
  }

  [Test]
  public void IsFirstOrLast_ReturnsTrue_WhenItemIsFirst()
  {
    // Arrange
    var list = new List<int>() { 1, 2, 3, 4, 5 };

    // Act
    var result = list.IsFirstOrLast(1);

    // Assert
    Assert.That(result, Is.True);
  }

  [Test]
  public void IsFirstOrLast_ReturnsTrue_WhenItemIsLast()
  {
    // Arrange
    var list = new List<int>() { 1, 2, 3, 4, 5 };

    // Act
    var result = list.IsFirstOrLast(5);

    // Assert
    Assert.That(result, Is.True);
  }

  [Test]
  public void IsFirstOrLast_ReturnsFalse_WhenItemIsMiddle()
  {
    // Arrange
    var list = new List<int>() { 1, 2, 3, 4, 5 };

    // Act
    var result = list.IsFirstOrLast(3);

    // Assert
    Assert.That(result, Is.False);
  }

  [Test]
  public void IsFirstOrLast_ReturnsFalse_WhenListIsEmpty()
  {
    // Arrange
    var list = new List<int>();

    // Act
    var result = list.IsFirstOrLast(1);

    // Assert
    Assert.That(result, Is.False);
  }

  [Test]
  public void IsFirstOrLast_ReturnsFalse_WhenItemNotInList()
  {
    // Arrange
    var list = new List<int>() { 1, 2, 3, 4, 5 };

    // Act
    var result = list.IsFirstOrLast(10);

    // Assert
    Assert.That(result, Is.False);
  }

  [Test]
  public void IsFirstOrLast_ReturnsTrue_WhenListHasOnlyOneItem()
  {
    // Arrange
    var list = new List<string>() { "Only Item" };

    // Act
    var result = list.IsFirstOrLast("Only Item");

    // Assert
    Assert.That(result, Is.True);
  }
}
