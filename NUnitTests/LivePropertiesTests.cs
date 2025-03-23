using System.Diagnostics.CodeAnalysis;
using TNT.Drawing;

namespace NUnitTests;

[ExcludeFromCodeCoverage]
internal class LivePropertiesTests
{
  private class Subclass : LiveProperties
  {
    public int IntProperty { get { return Get<int>(); } set { Set(value); } }
    public string StringProperty { get { return Get<string>() ?? string.Empty; } set { Set(value); } }
    public string? NullProperty { get { return Get<string?>(); } set { Set(value); } }

    public Subclass()
    {
      _BackingFields = new Dictionary<string, object?>
      {
        { "IntProperty", 0 },
        { "StringProperty", string.Empty }
      };
    }
  }

  [Test]
  public void Set_WhenValueIsDifferent_ShouldCallOnPropertyChanged()
  {
    var calledCount = 0;
    var sut = new Subclass();
    sut.OnPropertyChanged = (k, v) => calledCount++;
    sut.IntProperty = 1;
    sut.StringProperty = "Hello";
    sut.NullProperty = "Bye";
    sut.IntProperty = 1;
    sut.NullProperty = null;
    Assert.That(calledCount, Is.EqualTo(4));

    Assert.That(sut.IntProperty, Is.EqualTo(1));

    LiveProperties.Set(sut, "IntProperty", 2);
    LiveProperties.Set(sut, "InvalidProperty", 2);
    Assert.That(sut.IntProperty, Is.EqualTo(2));

  }
}
