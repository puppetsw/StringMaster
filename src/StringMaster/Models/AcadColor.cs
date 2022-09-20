using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace StringMaster.Models;

public sealed class AcadColor : ObservableObject, IEquatable<AcadColor>
{
    private byte _red;
    private byte _green;
    private byte _blue;
    private bool _isByLayer;
    private bool _isByBlock;
    private bool _isVisible = true;
    private int? _colorIndex;
    private Guid _id = Guid.NewGuid();

    public Guid Id => _id;

    [XmlIgnore]
    public string Name => GetColorName();

    public bool IsColorPicker { get; set; }

    [XmlIgnore]
    public SolidColorBrush ColorBrush => new(Color.FromRgb(Red, Green, Blue));

    public bool IsByLayer
    {
        get => _isByLayer;
        set => SetProperty(ref _isByLayer, value);
    }

    public bool IsByBlock
    {
        get => _isByBlock;
        set => SetProperty(ref _isByBlock, value);
    }

    public byte Red
    {
        get => _red;
        set
        {
            SetProperty(ref _red, value);
            NotifyPropertyChanged(nameof(Name));
        }
    }

    public byte Green
    {
        get => _green;
        set
        {
            SetProperty(ref _green, value);
            NotifyPropertyChanged(nameof(Name));
        }
    }

    public byte Blue
    {
        get => _blue;
        set
        {
            SetProperty(ref _blue, value);
            NotifyPropertyChanged(nameof(Name));
        }
    }

    public int? ColorIndex
    {
        get => _colorIndex;
        set
        {
            SetProperty(ref _colorIndex, value);
            NotifyPropertyChanged(nameof(Name));
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public AcadColor(byte red, byte green, byte blue, int? colorIndex = null)
    {
        Red = red;
        Green = green;
        Blue = blue;
        ColorIndex = colorIndex;
        NotifyPropertyChanged(nameof(ColorBrush));
        NotifyPropertyChanged(nameof(Name));
    }

    /// <summary>
    /// Parameter-less constructor for serialization.
    /// </summary>
    public AcadColor() { }

    public static AcadColor ByLayer => new(255, 255, 255) { IsByLayer = true };

    public static AcadColor ByBlock => new(255, 255, 255) { IsByBlock = true };

    /// <summary>
    /// Opens the ACAD color picker modal when this option is chosen in the list.
    /// </summary>
    public static AcadColor ColorPicker => new(255, 255, 255) { IsColorPicker = true };

    // TODO: Could remove this and use the ColorDisplayName from the Color object.
    private string GetColorName()
    {
        if (IsByLayer)
            return "ByLayer";

        if (IsByBlock)
            return "ByBlock";

        if (IsColorPicker)
            return "Select Color...";

        switch (Red, Green, Blue)
        {
            case (255, 0, 0):
                return "Red";
            case (255, 255, 0):
                return "Yellow";
            case (0, 255, 0):
                return "Green";
            case (0, 255, 255):
                return "Cyan";
            case (0, 0, 255):
                return "Blue";
            case (255, 0, 255):
                return "Magenta";
            case (255, 255, 255):
                return "White";
            case (0, 0, 0):
                return "Black";
            default:
                if (ColorIndex is not null)
                    return $"Color {ColorIndex}";
                return $"{Red},{Green},{Blue}";
        }
    }

    public bool Equals(AcadColor other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return _red == other._red && _green == other._green && _blue == other._blue && _isByLayer == other._isByLayer &&
               _isByBlock == other._isByBlock && _isVisible == other._isVisible && IsColorPicker == other.IsColorPicker;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is AcadColor other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public AcadColor Clone()
    {
        var color = new AcadColor(Red, Green, Blue, ColorIndex)
        {
            IsColorPicker = IsColorPicker,
            IsByBlock = IsByBlock,
            IsByLayer = IsByLayer,
            IsVisible = IsVisible
        };

        return color;
    }
}
