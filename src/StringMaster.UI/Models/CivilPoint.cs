namespace StringMaster.UI.Models;

public sealed class CivilPoint : ObservableObject
{
    private string _pointNumber;
    private string _pointName;
    private double _easting;
    private double _northing;
    private double _elevation;
    private string _rawDescription;
    private string _fullDescription;

    public string PointNumber
    {
        get => _pointNumber;
        set => SetProperty(ref _pointNumber, value);
    }

    public string PointName
    {
        get => _pointName;
        set => SetProperty(ref _pointName, value);
    }

    public double Easting
    {
        get => _easting;
        set => SetProperty(ref _easting, value);
    }

    public double Northing
    {
        get => _northing;
        set => SetProperty(ref _northing, value);
    }

    public double Elevation
    {
        get => _elevation;
        set => SetProperty(ref _elevation, value);
    }

    public string RawDescription
    {
        get => _rawDescription;
        set => SetProperty(ref _rawDescription, value);
    }

    public string FullDescription
    {
        get => _fullDescription;
        set => SetProperty(ref _fullDescription, value);
    }
}
