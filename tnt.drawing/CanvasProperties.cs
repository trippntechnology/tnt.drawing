using System;
using System.ComponentModel;
using System.Drawing;
using TNT.Reactive;

namespace TNT.Drawing;

/// <summary>
/// Properties that can be changed in a <see cref="CanvasPanel"/>
/// </summary>
public class CanvasProperties
{
  private BackingFields _BackingFields = new BackingFields();

  public Action<string, object?> OnPropertyChanged = (k, v) => { };

  /// <summary>
  /// Initializes default values
  /// </summary>
  public CanvasProperties()
  {
    _BackingFields.OnFieldChanged = OnPropertyChanged;
  }

  /// <summary>
  /// Background color of the <see cref="Canvas"/>
  /// </summary>
  [DisplayName("Back Color")]
  public Color BackColor { get { return _BackingFields.Get(Color.Blue); } set { _BackingFields.Set(value); } }

  /// <summary>
  /// Serializable for <see cref="BackColor"/>
  /// </summary>
  [Browsable(false)]
  public int SerializableBackColor { get { return BackColor.ToArgb(); } set { BackColor = Color.FromArgb(value); } }

  /// <summary>
  /// </summary>
  [DisplayName("Layer Height")]
  public int LayerHeight { get { return _BackingFields.Get(768); } set { _BackingFields.Set(value); } }

  /// <summary>
  /// </summary>
  [DisplayName("Layer Width")]
  public int LayerWidth { get { return _BackingFields.Get(1024); } set { _BackingFields.Set(value); } }

  /// <summary>
  /// ScalePercentage of the <see cref="Canvas"/>
  /// </summary>
  [DisplayName("Scale Percentage")]
  public int ScalePercentage { get { return _BackingFields.Get(100); } set { _BackingFields.Set(value); } }

  /// <summary>
  /// Indicates the snap interval
  /// </summary>
  [DisplayName("Snap Interval")]
  [ReadOnly(true)]
  public int SnapInterval { get { return _BackingFields.Get(10); } set { _BackingFields.Set(value); } }

  /// <summary>
  /// Indicates objects should be snapped to <see cref="SnapInterval"/>
  /// </summary>
  [DisplayName("Snap To Interval")]
  public bool SnapToInterval { get { return _BackingFields.Get(true); } set { _BackingFields.Set(value); } }
}