using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using TNT.Commons;
using TNT.Drawing.Layers;
using TNT.Drawing.Model;
using TNT.Drawing.Objects;

namespace NUnitTests;

[ExcludeFromCodeCoverage]
public class TransformationTests
{
  [Test]
  public void CanvasPoint_Transformations()
  {
    var initialPoint = new CanvasPoint(42, 99) { Id = "e6afd009-36bd-44ca-86da-8700a8cc3912" };
    var json = Json.serializeObject(initialPoint);
    Assert.That(json, Is.EqualTo("{\"$type\":\"TNT.Drawing.Objects.CanvasPoint, TNT.Drawing\",\"X\":42,\"Y\":99,\"Id\":\"e6afd009-36bd-44ca-86da-8700a8cc3912\"}"));

    var point = Json.deserializeJson<CanvasPoint>(json);
    Assert.That(initialPoint, Is.EqualTo(point));
  }

  [Test]
  public void CanvasPoint_List_Serialization()
  {
    // Arrange: create a list with CanvasPoint and possible subclass
    var points = new List<CanvasPoint>
    {
        new CanvasPoint(10, 20) { Id = "id-1" },
        new CanvasPoint(30, 40) { Id = "id-2" }
        // Add subclass instances here if available, e.g. new CanvasPointSubclass(...)
    };

    // Act: serialize and deserialize
    var json = Json.serializeObject(points);
    var deserialized = Json.deserializeJson<List<CanvasPoint>>(json);

    // Assert: check count and equality
    Assert.That(deserialized, Is.Not.Null);
    Assert.That(deserialized.Count, Is.EqualTo(points.Count));
    for (int i = 0; i < points.Count; i++)
    {
      Assert.That(deserialized[i], Is.EqualTo(points[i]));
    }
  }

  [Test]
  public void BezierPath_Serialization_RoundTrip()
  {
    // Arrange: create a BezierPath with some vertices and control points
    var path = new BezierPath();
    var v1 = new Vertex(10, 20) { Id = "v1" };
    var v2 = new Vertex(30, 40) { Id = "v2" };
    var v3 = new Vertex(50, 60) { Id = "v3" };
    path.AddVertex(v1);
    path.AddVertex(v2);
    path.AddVertex(v3);
    path.LineColor = Color.Red;
    path.FillColor = Color.Yellow;
    path.Width = 2;
    path.LineStyle = DashStyle.Dash;

    // Act: serialize and deserialize
    var json = Json.serializeObject(path);
    var deserialized = Json.deserializeJson<BezierPath>(json);

    // Assert: check key properties and points
    Assert.That(deserialized, Is.Not.Null);
    Assert.That(deserialized.LineARGB, Is.EqualTo(path.LineARGB));
    Assert.That(deserialized.FillARGB, Is.EqualTo(path.FillARGB));
    Assert.That(deserialized.Width, Is.EqualTo(path.Width));
    Assert.That(deserialized.LineStyle, Is.EqualTo(path.LineStyle));
  }

  [Test]
  public void CanvasLayer_Serialization_RoundTrip()
  {
    var layer = new CanvasLayer()
    {
      Name = "TestLayer",
      IsVisible = true,
      BackgroundColor = Color.AliceBlue,
      Width = 123,
      Height = 456
    };
    var json = Json.serializeObject(layer);
    var deserialized = Json.deserializeJson<TNT.Drawing.Layers.CanvasLayer>(json);
    Assert.That(deserialized, Is.Not.Null);
    Assert.That(deserialized.Name, Is.EqualTo(layer.Name));
    Assert.That(deserialized.IsVisible, Is.EqualTo(layer.IsVisible));
    Assert.That(deserialized.BackgroundColor, Is.EqualTo(layer.BackgroundColor));
    Assert.That(deserialized.Width, Is.EqualTo(layer.Width));
    Assert.That(deserialized.Height, Is.EqualTo(layer.Height));
  }

  [Test]
  public void GridLayer_Serialization_RoundTrip()
  {
    var gridLayer = new GridLayer()
    {
      Name = "GridLayer",
      IsVisible = true,
      Width = 200,
      Height = 100,
      LineColor = Color.DarkGray
    };
    var json = Json.serializeObject(gridLayer);
    var deserialized = Json.deserializeJson<TNT.Drawing.Layers.GridLayer>(json);
    Assert.That(deserialized, Is.Not.Null);
    Assert.That(deserialized.Name, Is.EqualTo(gridLayer.Name));
    Assert.That(deserialized.IsVisible, Is.EqualTo(gridLayer.IsVisible));
    Assert.That(deserialized.Width, Is.EqualTo(gridLayer.Width));
    Assert.That(deserialized.Height, Is.EqualTo(gridLayer.Height));
    Assert.That(deserialized.LineColor, Is.EqualTo(gridLayer.LineColor));
  }

  [Test]
  public void CanvasProperties_Serialization_RoundTrip()
  {
    var props = new TNT.Drawing.CanvasProperties
    {
      BackColor = Color.LightGreen,
      LayerHeight = 555,
      LayerWidth = 777,
      ScalePercentage = 80,
      SnapInterval = 15,
      SnapToInterval = false
    };
    var json = Json.serializeObject(props);
    var deserialized = Json.deserializeJson<TNT.Drawing.CanvasProperties>(json);
    Assert.That(deserialized, Is.Not.Null);
    Assert.That(deserialized.BackColorArgb, Is.EqualTo(props.BackColorArgb));
    Assert.That(deserialized.LayerHeight, Is.EqualTo(props.LayerHeight));
    Assert.That(deserialized.LayerWidth, Is.EqualTo(props.LayerWidth));
    Assert.That(deserialized.ScalePercentage, Is.EqualTo(props.ScalePercentage));
    Assert.That(deserialized.SnapInterval, Is.EqualTo(props.SnapInterval));
    Assert.That(deserialized.SnapToInterval, Is.EqualTo(props.SnapToInterval));
  }

  [Test]
  public void CanvasState_Serialization_RoundTrip()
  {
    var props = new TNT.Drawing.CanvasProperties
    {
      BackColor = Color.MediumPurple,
      LayerHeight = 600,
      LayerWidth = 800,
      ScalePercentage = 75,
      SnapInterval = 20,
      SnapToInterval = true
    };
    var state = new CanvasState(props, new List<CanvasLayer>());

    // Act: serialize and deserialize
    var json = Json.serializeObject(state);
    var deserialized = Json.deserializeJson<TNT.Drawing.Model.CanvasState>(json);

    // Assert: check that deserialized is not null and is of correct type
    Assert.That(deserialized, Is.Not.Null);
    Assert.That(deserialized, Is.InstanceOf<TNT.Drawing.Model.CanvasState>());
  }
}
