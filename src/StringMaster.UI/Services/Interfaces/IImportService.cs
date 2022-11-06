using System;
using System.Collections.Generic;
using System.IO;
using StringMaster.UI.Models;

namespace StringMaster.UI.Services.Interfaces;

public interface IImportService
{
    IEnumerable<CivilPoint> PointsFromFile(string fileName);
}

public class ImportService : IImportService
{
    public IEnumerable<CivilPoint> PointsFromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentNullException(nameof(fileName), "Filename was null or empty.");

        var file = new FileInfo(fileName);

        if (!file.Exists)
            throw new FileNotFoundException($"File not found at {fileName}");

        // var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        // {
        //     HasHeaderRecord = false,
        // };

        List<CivilPoint> pointList = new();

        // using var reader = new StreamReader(fileName);
        // using var csv = new CsvReader(reader, config);
        // {
        //     csv.Context.RegisterClassMap<CivilPointMap>();
        //     var points = csv.GetRecords<CivilPoint>();
        //     pointList = points.ToList();
        // }

        return pointList;
    }
}


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

