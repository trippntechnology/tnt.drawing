using System.Diagnostics.CodeAnalysis;
using TNT.Drawing.Extensions;

namespace NUnitTests.Extensions;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ObjectExtTests
{
  [Test]
  public void SetProperties_CopiesPropertiesCorrectly()
  {
    // Arrange
    var source = new SourceClass { Id = 1, Name = "Source" };
    var destination = new DestinationClass();

    // Act
    source.SetProperties(destination);

    // Assert
    Assert.That(source.Id, Is.EqualTo(destination.Id));
    Assert.That(source.Name, Is.EqualTo(destination.Name));
  }

  [Test]
  public void SetProperties_DoesNotCopyReadOnlyProperties()
  {
    // Arrange
    var source = new SourceClassWithReadOnly { Id = 1, Name = "Source", ReadOnlyProperty = "ReadOnly" };
    var destination = new DestinationClassWithReadOnly();

    // Act
    source.SetProperties(destination);

    // Assert
    Assert.That(source.Id, Is.EqualTo(destination.Id));
    Assert.That(source.Name, Is.EqualTo(destination.Name));
    Assert.That(source.ReadOnlyProperty, Is.Not.EqualTo(destination.ReadOnlyProperty));
  }
  public class SourceClass
  {
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool Condition { get; set; }
  }

  public class DestinationClass
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
  }

  public class SourceClassWithReadOnly
  {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ReadOnlyProperty { get; set; }
  }

  public class DestinationClassWithReadOnly
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReadOnlyProperty { get; } = "Initial Value";
  }
}
