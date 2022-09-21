namespace StringMaster.Models;

public abstract class LayerPropertyBase : ObservableObject
{
    private string _name;
    private object _value;
    private bool _isReadOnly;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public object Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => SetProperty(ref _isReadOnly, value);
    }
}

public class ColorProperty : LayerPropertyBase { }

public class SelectYesNoProperty : LayerPropertyBase { }

public class StringProperty : LayerPropertyBase { }
