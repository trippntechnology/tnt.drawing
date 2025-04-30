using System.Diagnostics.CodeAnalysis;
using TNT.Drawing.Extensions;

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
}
