using System;

namespace StringMaster.Models;

public sealed class DescriptionKey : ObservableObject, ICloneable, IEquatable<DescriptionKey>
{
    private string _description;
    private bool _draw2D;
    private bool _draw3D;
    private bool _drawFeatureLine;
    private string _key;
    private string _layer = "0";
    private double _midOrdinate = 0.01;

    /// <summary>
    /// Gets the key value.
    /// </summary>
    /// <remarks>Always returns in Uppercase.</remarks>
    public string Key
    {
        get => _key?.ToUpperInvariant();
        set => SetProperty(ref _key, value);
    }

    public string Layer
    {
        get => _layer;
        set => SetProperty(ref _layer, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

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

    public bool IsValid() => !string.IsNullOrEmpty(_key) && !string.IsNullOrEmpty(Layer);

    public object Clone() => MemberwiseClone();

    public bool Equals(DescriptionKey other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return _description == other._description && _draw2D == other._draw2D && _draw3D == other._draw3D &&
               _drawFeatureLine == other._drawFeatureLine && _key == other._key && _layer == other._layer &&
               _midOrdinate.Equals(other._midOrdinate);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is DescriptionKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Description != null ? Description.GetHashCode() : 0;
            hashCode = hashCode * 397 ^ Draw2D.GetHashCode();
            hashCode = hashCode * 397 ^ Draw3D.GetHashCode();
            hashCode = hashCode * 397 ^ DrawFeatureLine.GetHashCode();
            hashCode = hashCode * 397 ^ (Key != null ? Key.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (Layer != null ? Layer.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ MidOrdinate.GetHashCode();
            return hashCode;
        }
    }
}
