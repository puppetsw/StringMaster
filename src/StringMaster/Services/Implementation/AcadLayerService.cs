#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using StringMaster.Extensions;
using StringMaster.Helpers;
using StringMaster.UI.Models;
using StringMaster.UI.Services.Interfaces;

namespace StringMaster.Services.Implementation;

public class AcadLayerService : IAcadLayerService
{
    public IEnumerable<AcadLayer> GetLayersFromActiveDocument()
    {
        var document = AcadApplicationService.DocumentManager.MdiActiveDocument;
        return GetLayersFromDocument(document.Name);
    }

    public IEnumerable<AcadLayer> GetLayersFromDocument(string? documentName)
    {
        Document document = null!;
        foreach (Document doc in AcadApplicationService.DocumentManager)
        {
            if (doc.Name != documentName)
                continue;

            document = doc;
        }

        if (document is null)
            throw new InvalidOperationException("document was null.");

        var layerList = new List<AcadLayer>();

        // Start locked transaction on the correct document.
        using var tr = document.TransactionManager.StartLockedTransaction();

        var layerTable = (LayerTable)tr.GetObject(document.Database.LayerTableId, OpenMode.ForRead);

        foreach (var objectId in layerTable)
        {
            var layer = (LayerTableRecord)tr.GetObject(objectId, OpenMode.ForRead);
            var color = layer.Color.ToAcadColor();
            var lineType = (LinetypeTableRecord)tr.GetObject(layer.LinetypeObjectId, OpenMode.ForRead);

            string lineWeightText = layer.LineWeight switch
            {
                LineWeight.ByLayer => "ByLayer",
                LineWeight.ByBlock => "ByBlock",
                LineWeight.ByLineWeightDefault => "Default",
                _ => layer.LineWeight.ToString()
            };

            var acadLayer = new AcadLayer(layer.Name, layer.IsOff, layer.IsFrozen, layer.IsLocked, color)
            {
                IsPlottable = layer.IsPlottable,
                Lineweight = lineWeightText,
                Linetype = lineType.Name,
                PlotStyleName = layer.PlotStyleName
            };

            layerList.Add(acadLayer);
        }

        tr.Commit();

        return layerList.OrderBy(x => x.Name);
    }

    public void CreateLayer(AcadLayer? layer, string? documentName = null)
    {
        if (layer == null)
            return;

        Document document = AcadApplicationService.DocumentManager.MdiActiveDocument;

        if (!string.IsNullOrEmpty(documentName))
        {
            foreach (Document doc in AcadApplicationService.DocumentManager)
            {
                if (doc.Name != documentName)
                    continue;

                document = doc;
                break;
            }
        }

        using var tr = document.TransactionManager.StartLockedTransaction();

        if (!LayerHelpers.HasLayer(layer.Name, tr, document.Database))
            LayerHelpers.CreateLayer(layer, tr, document.Database);

        tr.Commit();
    }
}
