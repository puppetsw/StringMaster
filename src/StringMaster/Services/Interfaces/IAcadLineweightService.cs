using System.Collections.Generic;
using StringMaster.Models;

namespace StringMaster.Services.Interfaces;

public interface IAcadLineweightService
{
    IEnumerable<AcadLineweight> GetLineweightsFromActiveDocument();
}
