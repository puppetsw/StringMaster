using System;
using System.Xml.Serialization;

namespace StringMaster.UI.Models;

public sealed class DescriptionKey : ObservableObject, IEquatable<DescriptionKey>
{
    private Guid _id = Guid.NewGuid();

    public Guid Id => _id;

    private bool _draw2D;
    private bool _draw3D;
    private bool _drawFeatureLine;
    private string _key;
    private string _layer = "0";
    private double _midOrdinate = 0.01;
    private AcadColor _acadColor = AcadColor.ByLayer;
    private AcadLayer _acadLayer = new();

    /// <summary>
    /// Gets the key value.
    /// </summary>
    /// <remarks>Always returns in Uppercase.</remarks>
    public string Key
    {
        get => _key?.ToUpperInvariant();
        set => SetProperty(ref _key, value);
    }

    [XmlIgnore]
    public string Layer => AcadLayer.Name;

    public bool Draw2D
    {
        get => _draw2D;
        set => SetProperty(ref _draw2D, value);
    }

    public bool Draw3D
    {
        get => _draw3D;
        set => SetProperty(ref _draw3D, value);
    }

    public bool DrawFeatureLine
    {
        get => _drawFeatureLine;
        set => SetProperty(ref _drawFeatureLine, value);
    }

    public double MidOrdinate
    {
        get => _midOrdinate;
        set => SetProperty(ref _midOrdinate, value);
    }

    public AcadColor AcadColor
    {
        get => _acadColor;
        set => SetProperty(ref _acadColor, value);
    }

    public AcadLayer AcadLayer
    {
        get => _acadLayer;
        set => SetProperty(ref _acadLayer, value);
    }

    public bool IsValid => !string.IsNullOrEmpty(_key) && !string.IsNullOrEmpty(Layer);

    public bool Equals(DescriptionKey other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return _draw2D == other._draw2D && _draw3D == other._draw3D && _drawFeatureLine == other._drawFeatureLine &&
               _key == other._key && _layer == other._layer && _midOrdinate.Equals(other._midOrdinate) &&
               Equals(_acadColor, other._acadColor) && Equals(_acadLayer, other._acadLayer);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is DescriptionKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public DescriptionKey Clone()
    {
        var key = new DescriptionKey
        {
            AcadLayer = AcadLayer.Clone(),
            AcadColor = AcadColor.Clone(),
            Key = Key,
            DrawFeatureLine = DrawFeatureLine,
            Draw3D = Draw3D,
            Draw2D = Draw2D,
            MidOrdinate = MidOrdinate
        };

        return key;
    }
}
