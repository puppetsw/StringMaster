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

        List<CivilPoint> pointList = new();

        using var reader = new StreamReader(fileName);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (string.IsNullOrEmpty(line))
                continue;

            var values = line.Split(',');

            pointList.Add(new CivilPoint
            {
                PointNumber = values[0],
                Easting = Convert.ToDouble(values[1]),
                Northing = Convert.ToDouble(values[2]),
                Elevation = Convert.ToDouble(values[3]),
                RawDescription = values[4]
            });
        }

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

