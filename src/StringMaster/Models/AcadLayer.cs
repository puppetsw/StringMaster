using System;
using System.Xml.Serialization;

namespace StringMaster.Models;

public sealed class AcadLayer : ObservableObject
{
    private Guid _id = Guid.NewGuid();

    public Guid Id => _id;

    private string _name;
    private bool _isOn;
    private bool _isFrozen;
    private bool _isLocked;
    private AcadColor _color;
    private bool _isPlottable;
    private string _lineWeight;
    private string _lineType;
    private string _plotStyleName;
    private bool _isSelected;

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

    public string ColorStringLower => Color.Name.ToLower();

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

    public bool IsPlottable
    {
        get => _isPlottable;
        set => SetProperty(ref _isPlottable, value);
    }

    public string LineWeight
    {
        get => _lineWeight;
        set => SetProperty(ref _lineWeight, value);
    }

    public string Linetype
    {
        get => _lineType;
        set => SetProperty(ref _lineType, value);
    }

    public string PlotStyleName
    {
        get => _plotStyleName;
        set => SetProperty(ref _plotStyleName, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public AcadLayer() // For Serialization
    {
        Name = "0";
        Color = AcadColor.ByLayer;
        _isOn = true;
        _isFrozen = false;
        _isLocked = false;
    }

    public AcadLayer(string name, bool isOn, bool isFrozen, bool isLocked, AcadColor color)
    {
        _name = name;
        _isOn = isOn;
        _isFrozen = isFrozen;
        _isLocked = isLocked;
        _color = color;
    }

    [XmlIgnore]
    public bool IsValid => !string.IsNullOrEmpty(Name);

    public bool Equals(AcadLayer other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return _name == other._name && _isOn == other._isOn && _isFrozen == other._isFrozen &&
               _isLocked == other._isLocked && Equals(_color, other._color) && _isPlottable == other._isPlottable &&
               _lineWeight == other._lineWeight && _lineType == other._lineType &&
               _plotStyleName == other._plotStyleName && _isSelected == other._isSelected;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is AcadLayer other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
