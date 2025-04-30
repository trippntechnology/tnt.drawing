using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using TNT.Drawing.Extensions;

namespace TNT.Drawing.Tests.Extensions;

[ExcludeFromCodeCoverage]
[TestFixture]
public class MouseEventArgsExtTests
{
  [Test]
  public void HasButtonDown_WhenLeftButtonIsPressed_ReturnsTrue()
  {
    // Arrange
    var mouseEventArgs = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);

    // Act
    var result = mouseEventArgs.HasButtonDown();

    // Assert
    Assert.That(result, Is.True);
  }

  [Test]
  public void HasButtonDown_WhenRightButtonIsPressed_ReturnsTrue()
  {
    // Arrange
    var mouseEventArgs = new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0);

    // Act
    var result = mouseEventArgs.HasButtonDown();

    // Assert
    Assert.That(result, Is.True);
  }

  [Test]
  public void HasButtonDown_WhenMiddleButtonIsPressed_ReturnsTrue()
  {
    // Arrange
    var mouseEventArgs = new MouseEventArgs(MouseButtons.Middle, 1, 0, 0, 0);

    // Act
    var result = mouseEventArgs.HasButtonDown();

    // Assert
    Assert.That(result, Is.True);
  }

  [Test]
  public void HasButtonDown_WhenNoButtonIsPressed_ReturnsFalse()
  {
    // Arrange
    var mouseEventArgs = new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0);

    // Act
    var result = mouseEventArgs.HasButtonDown();

    // Assert
    Assert.That(result, Is.False);
  }
}
