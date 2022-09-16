using System;
using System.Windows.Media;

namespace StringMaster.Models;

public sealed class AcadColor : ObservableObject, IEquatable<AcadColor>
{
    private byte _red;
    private byte _green;
    private byte _blue;
    private bool _isByLayer;
    private bool _isByBlock;
    private bool _isVisible = true;

    public string Name
    {
        get
        {
            if (IsByLayer)
                return "ByLayer";

            if (IsByBlock)
                return "ByBlock";

            if (IsColorPicker)
                return "Select Color...";

            return $"{Red},{Green},{Blue}";
        }
    }

    public bool IsColorPicker { get; set; }

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
            NotifyPropertyChanged(nameof(Blue));
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public AcadColor(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
        NotifyPropertyChanged(nameof(ColorBrush));
        NotifyPropertyChanged(nameof(Name));
    }

    public bool Equals(AcadColor other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Red == other.Red && Green == other.Green && Blue == other.Blue;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is AcadColor other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Name != null ? Name.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ Red.GetHashCode();
            hashCode = (hashCode * 397) ^ Green.GetHashCode();
            hashCode = (hashCode * 397) ^ Blue.GetHashCode();
            return hashCode;
        }
    }
}
