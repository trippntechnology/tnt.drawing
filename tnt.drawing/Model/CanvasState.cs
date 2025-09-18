using System.Collections.Generic;
using TNT.Drawing.Layers;

namespace TNT.Drawing.Model;

/// <summary>
/// Represents the state of the canvas, including its properties.
/// </summary>
/// <param name="properties">The properties of the canvas, such as background color, layer dimensions, scale, and snapping options.</param>
/// <param name="layers">The list of layers currently present on the canvas, each containing drawable objects and layer-specific properties.</param>
public record CanvasState(CanvasProperties properties, List<CanvasLayer> layers);
