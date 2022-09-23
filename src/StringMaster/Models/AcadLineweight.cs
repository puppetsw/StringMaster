namespace StringMaster.Models;

public sealed class AcadLineweight : ObservableObject
{
    private double _weightValue;
    private string _name;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public double WeightValue
    {
        get => _weightValue;
        set => SetProperty(ref _weightValue, value);
    }
}
