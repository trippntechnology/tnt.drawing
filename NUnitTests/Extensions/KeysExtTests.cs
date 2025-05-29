using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using NUnit.Framework;
using TNT.Drawing.Extensions;

namespace NUnitTests.Extensions;

[ExcludeFromCodeCoverage]
public class KeysExtTests
{
    [Test]
    public void ContainsAll_SingleKey_ReturnsTrue()
    {
        // Arrange
        var keys = Keys.Control;
        
        // Act
        bool result = keys.ContainsAll(Keys.Control);
        
        // Assert
        Assert.That(result, Is.True);
    }    [Test]
    public void ContainsAll_MultipleKeysAllPresent_ReturnsTrue()
    {
        // Arrange
        var keys = Keys.Control | Keys.Shift | Keys.Alt;
        
        // Act
        bool result = keys.ContainsAll(Keys.Control, Keys.Shift);
        
        // Assert
        Assert.That(result, Is.True);
    }    [Test]
    public void ContainsAll_MultipleKeysNotAllPresent_ReturnsFalse()
    {
        // Arrange
        var keys = Keys.Control | Keys.Alt;
        
        // Act
        bool result = keys.ContainsAll(Keys.Control, Keys.Shift);
        
        // Assert
        Assert.That(result, Is.False);
    }    [Test]
    public void ContainsAll_EmptyKeysToCheck_ReturnsTrue()
    {
        // Arrange
        var keys = Keys.Control;
        
        // Act
        bool result = keys.ContainsAll();
        
        // Assert
        Assert.That(result, Is.True);
    }    [Test]
    public void ContainsAny_SingleKeyPresent_ReturnsTrue()
    {
        // Arrange
        var keys = Keys.Control;
        
        // Act
        bool result = keys.ContainsAny(Keys.Control);
        
        // Assert
        Assert.That(result, Is.True);
    }    [Test]
    public void ContainsAny_OneOfMultipleKeysPresent_ReturnsTrue()
    {
        // Arrange
        var keys = Keys.Control | Keys.Alt;
        
        // Act
        bool result = keys.ContainsAny(Keys.Shift, Keys.Control);
        
        // Assert
        Assert.That(result, Is.True);
    }    [Test]
    public void ContainsAny_NoneOfMultipleKeysPresent_ReturnsFalse()
    {
        // Arrange
        var keys = Keys.Control | Keys.Alt;
        
        // Act
        bool result = keys.ContainsAny(Keys.Shift, Keys.Tab);
        
        // Assert
        Assert.That(result, Is.False);
    }    [Test]
    public void ContainsAny_EmptyKeysToCheck_ReturnsFalse()
    {
        // Arrange
        var keys = Keys.Control;
        
        // Act
        bool result = keys.ContainsAny();
        
        // Assert
        Assert.That(result, Is.False);
    }    [Test]
    public void ContainsAll_WithModifierKeys_ReturnsTrue()
    {
        // Arrange
        var keys = Keys.Control | Keys.A;
        
        // Act
        bool result = keys.ContainsAll(Keys.Control, Keys.A);
        
        // Assert
        Assert.That(result, Is.True);
    }    [Test]
    public void ContainsAny_WithModifierKeys_ReturnsTrue()
    {
        // Arrange
        var keys = Keys.Control | Keys.A;
        
        // Act
        bool result = keys.ContainsAny(Keys.B, Keys.A);
        
        // Assert
        Assert.That(result, Is.True);
    }
}
