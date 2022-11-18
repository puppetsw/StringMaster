using System.Linq;
using NUnit.Framework;
using StringMaster.UI.Services.Interfaces;

namespace StringMaster.Tests;

[TestFixture]
public class ImportServiceTests
{
    [Test]
    public void SimpleImport_Success()
    {
        const string fileName = "sample files\\valid_import_file.csv";
        IImportService importService = new ImportService();
        var points = importService.PointsFromFile(fileName, out _);
        Assert.IsTrue(points.Any());
    }

    [Test]
    public void SimpleImport_Invalid_file()
    {
        const string fileName = "sample files\\invalid_import_file_row1.csv";
        IImportService importService = new ImportService();
        var points = importService.PointsFromFile(fileName, out _);
        Assert.IsTrue(points.Any());
    }
}

