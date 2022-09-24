#nullable enable

using System.Collections.Generic;
using StringMaster.Models;

namespace StringMaster.Services.Interfaces;

public interface IAcadLayerService
{
    IEnumerable<AcadLayer> GetLayersFromActiveDocument();
    IEnumerable<AcadLayer> GetLayersFromDocument(string? documentName);
    void CreateLayer(AcadLayer? layer, string? documentName = null);
}
