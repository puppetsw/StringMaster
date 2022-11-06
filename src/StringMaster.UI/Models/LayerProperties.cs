namespace StringMaster.UI.Models;

public abstract class PropertyBase : ObservableObject
{
    private string _name;
    private bool _isReadOnly;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => SetProperty(ref _isReadOnly, value);
    }

    protected PropertyBase(string name, bool isReadOnly = false)
    {
        Name = name;
        IsReadOnly = isReadOnly;
    }
}

public sealed class LayerNameProperty : PropertyBase { public LayerNameProperty() : base("Layer name") { } }
public sealed class LayerColorProperty : PropertyBase { public LayerColorProperty() : base("Color") { } }
public sealed class LayerLinetypeProperty : PropertyBase { public LayerLinetypeProperty() : base("Linetype") { } }
public sealed class LayerLineweightProperty : PropertyBase { public LayerLineweightProperty() : base("Lineweight") { } }
public sealed class LayerLockedProperty : PropertyBase { public LayerLockedProperty() : base("Locked") { } }
public sealed class LayerOnProperty : PropertyBase { public LayerOnProperty() : base("On") { } }
public sealed class LayerFrozenProperty : PropertyBase { public LayerFrozenProperty() : base("Frozen") { } }
public sealed class LayerPlotStyleProperty : PropertyBase { public LayerPlotStyleProperty() : base("Plot Style", true) { } }
public sealed class LayerPlotProperty : PropertyBase { public LayerPlotProperty() : base("Plot") { } }
