#nullable enable

using System.Collections.Generic;
using StringMaster.UI.Models;

namespace StringMaster.UI.Services.Interfaces;

public interface IAcadLayerService
{
    IEnumerable<AcadLayer> GetLayersFromActiveDocument();
    IEnumerable<AcadLayer> GetLayersFromDocument(string? documentName);
    void CreateLayer(AcadLayer? layer, string? documentName = null);
}
