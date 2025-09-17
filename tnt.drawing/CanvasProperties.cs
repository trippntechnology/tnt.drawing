using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;
using TNT.Reactive;

namespace TNT.Drawing;

/// <summary>
/// Properties that can be changed in a <see cref="CanvasPanel"/>
/// </summary>
public class CanvasProperties() : Observable()
{
  /// <summary>
  /// Background color of the <see cref="Canvas"/>
  /// </summary>
  [DisplayName("Back Color")]
  [JsonIgnore]
  public Color BackColor { get { return Get(Color.Blue); } set { Set(value); } }

  /// <summary>
  /// Serializable for <see cref="BackColor"/>
  /// </summary>
  [Browsable(false)]
  public int BackColorArgb { get { return BackColor.ToArgb(); } set { BackColor = Color.FromArgb(value); } }

  /// <summary>
  /// </summary>
  [DisplayName("Layer Height")]
  public int LayerHeight { get { return Get(768); } set { Set(value); } }

  /// <summary>
  /// </summary>
  [DisplayName("Layer Width")]
  public int LayerWidth { get { return Get(1024); } set { Set(value); } }

  /// <summary>
  /// ScalePercentage of the <see cref="Canvas"/>
  /// </summary>
  [DisplayName("Scale Percentage")]
  public int ScalePercentage { get { return Get(100); } set { Set(value); } }

  /// <summary>
  /// Indicates the snap interval
  /// </summary>
  [DisplayName("Snap Interval")]
  [ReadOnly(true)]
  public int SnapInterval { get { return Get(10); } set { Set(value); } }

  /// <summary>
  /// Indicates objects should be snapped to <see cref="SnapInterval"/>
  /// </summary>
  [DisplayName("Snap To Interval")]
  public bool SnapToInterval { get { return Get(true); } set { Set(value); } }
}