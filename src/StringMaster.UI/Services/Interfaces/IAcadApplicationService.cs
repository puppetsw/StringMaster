using System.Collections.Generic;
using StringMaster.UI.Models;

namespace StringMaster.UI.Services.Interfaces;

public interface IAcadApplicationService
{
    IEnumerable<AcadDocument> Documents { get; }

    AcadDocument ActiveDocument { get; }

    void WriteMessage(string message);
}