using System.Collections.Generic;
using StringMaster.Models;
using StringMaster.Services.Interfaces;

namespace StringMaster.Services.Implementation;

public class AcadLineweightService : IAcadLineweightService
{
    // TODO: Can these lineweights remain hardcoded?
    public IEnumerable<AcadLineweight> GetLineweightsFromActiveDocument()
    {
        var list = new List<AcadLineweight>
        {
            new() { Name = "ByLayer", WeightValue = 0 },
            new() { Name = "ByBlock", WeightValue = 0 },
            new() { Name = "Default", WeightValue = 0 },
            new() { Name = "0.00 mm", WeightValue = 0 },
            new() { Name = "0.05 mm", WeightValue = 0.05 },
            new() { Name = "0.09 mm", WeightValue = 0.09 },
            new() { Name = "0.13 mm", WeightValue = 0.13 },
            new() { Name = "0.15 mm", WeightValue = 0.15 },
            new() { Name = "0.18 mm", WeightValue = 0.18 },
            new() { Name = "0.20 mm", WeightValue = 0.20 },
            new() { Name = "0.25 mm", WeightValue = 0.25 },
            new() { Name = "0.30 mm", WeightValue = 0.30 },
            new() { Name = "0.35 mm", WeightValue = 0.35 },
            new() { Name = "0.40 mm", WeightValue = 0.40 },
            new() { Name = "0.50 mm", WeightValue = 0.50 },
            new() { Name = "0.53 mm", WeightValue = 0.53 },
            new() { Name = "0.60 mm", WeightValue = 0.60 },
            new() { Name = "0.70 mm", WeightValue = 0.70 },
            new() { Name = "0.80 mm", WeightValue = 0.80 },
            new() { Name = "0.90 mm", WeightValue = 0.90 },
            new() { Name = "1.00 mm", WeightValue = 1.00 },
            new() { Name = "1.06 mm", WeightValue = 1.06 },
            new() { Name = "1.20 mm", WeightValue = 1.20 },
            new() { Name = "1.40 mm", WeightValue = 1.40 },
            new() { Name = "1.58 mm", WeightValue = 1.58 },
            new() { Name = "2.00 mm", WeightValue = 2.00 },
            new() { Name = "2.11 mm", WeightValue = 2.11 }
        };
        return list;
    }
}
