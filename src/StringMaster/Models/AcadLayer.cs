using System;

namespace StringMaster.Models;

public sealed class AcadLayer : ObservableObject, IEquatable<AcadLayer>
{
    private string _name;
    private bool _isOn;
    private bool _isFrozen;
    private bool _isLocked;
    private AcadColor _color;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public AcadColor Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    public bool IsOn
    {
        get => _isOn;
        set => SetProperty(ref _isOn, value);
    }

    public bool IsFrozen
    {
        get => _isFrozen;
        set => SetProperty(ref _isFrozen, value);
    }

    public bool IsLocked
    {
        get => _isLocked;
        set => SetProperty(ref _isLocked, value);
    }

    public AcadLayer()
    {
        Name = "0";
        _isOn = true;
        _isFrozen = false;
        _isLocked = false;
    }

    public bool Equals(AcadLayer other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return _name == other._name && _isOn == other._isOn && _isFrozen == other._isFrozen &&
               _isLocked == other._isLocked && Equals(_color, other._color);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is AcadLayer other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Name != null ? Name.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ IsOn.GetHashCode();
            hashCode = (hashCode * 397) ^ IsFrozen.GetHashCode();
            hashCode = (hashCode * 397) ^ IsLocked.GetHashCode();
            hashCode = (hashCode * 397) ^ (Color != null ? Color.GetHashCode() : 0);
            return hashCode;
        }
    }
}
