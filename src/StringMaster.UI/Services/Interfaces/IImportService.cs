using System;
using System.Collections.Generic;
using System.IO;
using StringMaster.UI.Models;

namespace StringMaster.UI.Services.Interfaces;

public interface IImportService
{
    IEnumerable<CivilPoint> PointsFromFile(string fileName, out IList<string> errorMessages);
}

public class ImportService : IImportService
{
    public IEnumerable<CivilPoint> PointsFromFile(string fileName, out IList<string> errorMessages)
    {
        errorMessages = new List<string>();

        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentNullException(nameof(fileName), "Filename was null or empty.");

        var file = new FileInfo(fileName);

        if (!file.Exists)
            throw new FileNotFoundException($"File not found at {fileName}");

        List<CivilPoint> pointList = new();
        var currentLine = 0;
        using var reader = new StreamReader(fileName);

        while (!reader.EndOfStream)
        {
            currentLine++;
            var line = reader.ReadLine();

            if (string.IsNullOrEmpty(line))
                continue;

            var values = line.Split(',');

            try
            {
                pointList.Add(new CivilPoint
                {
                    PointNumber = values[0],
                    Easting = Convert.ToDouble(values[1]),
                    Northing = Convert.ToDouble(values[2]),
                    Elevation = Convert.ToDouble(values[3]),
                    RawDescription = values[4]
                });
            }
            catch (FormatException e)
            {
                errorMessages.Add($"Line: {currentLine}, Format error.");
            }
        }

        return pointList;
    }
}
