# TNT.Drawing AI Coding Agent Instructions

## Project Overview
TNT.Drawing is a .NET 9 Windows Forms/WPF drawing library providing a layered canvas system with interactive drawing modes. The library follows a graphics coordinate transformation architecture where mouse events are transformed between canvas and grid coordinate spaces.

## Architecture Overview

### Core Components
- **Canvas**: Main drawing surface (`Canvas.cs`) - manages layers, handles mouse/keyboard events, coordinate transformations, scroll behavior
- **CanvasPanel**: Parent control (`CanvasPanel.cs`) - prevents scroll reset on focus, provides container for Canvas
- **CanvasLayer**: Rendering layers (`Layers/CanvasLayer.cs`) - contains collections of drawable objects with BackingFields property management
- **CanvasObject**: Base class for all drawable items (`Objects/CanvasObject.cs`) - abstract with required Draw(), Clone(), Align() methods
- **DrawingMode**: Interactive behavior controllers (`DrawingModes/DrawingMode.cs`) - handle user input for specific drawing operations

### Key Patterns

#### Coordinate System Transformations
The library uses Graphics.TransformPoints() for coordinate space conversions:
```csharp
// Grid coordinates (world space) ↔ Canvas coordinates (screen space)
point.ToGridCoordinates(graphics)  // Screen → World
point.ToCanvasCoordinates(graphics) // World → Screen
```

#### Drawing Mode Strategy Pattern
Each drawing mode (SelectMode, LineMode, SquareMode, RotationMode) inherits from `DrawingMode` and overrides mouse/keyboard event handlers. Set active mode via `canvas.DrawingMode = mode`.

#### Layer-Based Rendering
Objects are organized in layers with z-order rendering. GridLayer provides visual grid, other layers contain drawable objects. Selected objects are rendered after unselected ones within each layer.

#### Observable Pattern for Properties
Uses `TNT.Reactive.Observable` base class with `Get<T>()` and `Set<T>()` for property change notifications. CanvasProperties and CanvasLayer use `BackingFields` pattern.

## Development Workflows

### Building & Testing
```powershell
dotnet build                    # Build solution
dotnet test                     # Run NUnit tests
dotnet test --verbosity minimal # Cleaner test output
```

### Project Structure
- `tnt.drawing/` - Main library (TNT.Drawing.csproj)
- `NUnitTests/` - Unit tests with Extensions/ and Objects/ subdirectories
- `Sample/` - WinForms demo application showing library usage

### Testing Patterns
- NUnit framework with `[Test]`, `[SetUp]`, `[TearDown]` attributes
- Extension method tests in `NUnitTests/Extensions/`
- Object behavior tests in `NUnitTests/Objects/`
- Graphics transformations tested with mock Graphics objects

## Project-Specific Conventions

### Extension Methods (`Extensions/`)
Heavy use of extension methods for common operations:
- `PointExt.cs`: Coordinate transformations, snapping, arithmetic
- `ObjectExt.cs`: Common object operations
- `KeysExt.cs`, `MouseEventArgsExt.cs`: Input handling helpers

### Response Objects (`Model/`)
Event handling uses response record types:
- `MouseOverResponse(CanvasObject? HitObject)` - hit detection results
- `MouseDownResponse(CanvasObject? HitObject, CanvasObject? ChildObject, bool AllowMove)` - click handling
- `MouseUpResponse` - button release handling  
- `Feedback(Cursor, string)` - UI feedback with cursor and status text

### Resource Management
Embedded resources in `Resource/` (cursors, images) accessed via assembly reflection. Custom cursors for drawing operations stored as `.cur` files.

### Property Management
Uses `TNT.Reactive.Observable` base with `Get<T>(defaultValue)` and `Set<T>(value)` pattern:
```csharp
public Color BackgroundColor { 
    get => Get<Color>(Color.Transparent); 
    set => Set(value); 
}
```
CanvasLayer uses `_BackingFields` for property storage with change notifications that trigger `_Redraw = true`.

## Key Integration Points

### TNT.* Dependencies
- `TNT.Commons`: Core utilities and extensions
- `TNT.Reactive`: Observable pattern implementation  
- `TNT.Utilities`: Serialization and reflection helpers

### Windows Forms Integration
- Custom controls inherit from WinForms Control/Panel
- DoubleBuffered rendering for smooth graphics
- ScrollableControl integration for panning/zooming

### Graphics API Usage
- `Graphics.TransformPoints()` for coordinate conversions
- `Matrix` transformations for scaling/translation
- `SmoothingMode.AntiAlias` for quality rendering

## Common Gotchas

### Mouse Event Transformation
Always transform mouse coordinates before processing in drawing modes:
```csharp
var mea = Transform(e, graphics); // Convert to grid coordinates
```

### Layer Refresh Pattern
Call `canvas.Refresh(layer)` or `canvas.Invalidate()` after object modifications to trigger repaints.

### Interface Implementation
Objects implementing `IRotatable` must provide `GetCentroid()` and `RotateBy()` methods.

### Serialization Support
CanvasObjects should be serializable - avoid non-serializable field types or mark with `[NonSerialized]`.

### CanvasPanel vs Canvas
Canvas must be wrapped in CanvasPanel to prevent scroll position reset on focus. Canvas constructor automatically creates CanvasPanel parent.

### Space Key Pan Mode
Space key activates pan mode with hand cursor - handled in Canvas.OnKeyDown/OnKeyUp, bypasses DrawingMode when active.

## When to Ask for Additional Information

If you encounter scenarios not covered in these instructions, ask the user for clarification on:

### Project-Specific Context
- **Performance requirements**: Graphics rendering performance expectations, target frame rates
- **Platform constraints**: Specific Windows Forms/WPF version requirements or limitations
- **Coordinate system edge cases**: Custom transformation scenarios beyond standard grid/canvas mapping

### Implementation Guidance
- **Drawing mode behavior**: Expected user interaction patterns for new drawing modes
- **Object serialization**: Specific serialization requirements for custom CanvasObject types
- **Resource management**: Custom cursor/image resource handling patterns

### Integration Requirements
- **External dependencies**: Integration with other TNT.* libraries beyond Commons/Reactive/Utilities
- **Event handling**: Custom event patterns beyond standard mouse/keyboard interactions
- **Testing approaches**: Specific testing patterns for graphics-based functionality

### Debugging & Diagnostics
- **Visual debugging**: Tools or techniques for debugging coordinate transformations
- **Performance profiling**: Specific graphics performance monitoring approaches
- **Error handling**: Project-specific error handling patterns for graphics operations

Always provide context about what you're trying to implement when asking for guidance.
