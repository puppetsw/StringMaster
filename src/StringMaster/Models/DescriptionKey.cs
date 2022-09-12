namespace StringMaster.Models;

public class DescriptionKey : ObservableObject
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
}
