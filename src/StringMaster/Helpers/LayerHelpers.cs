using System;
using Autodesk.AutoCAD.DatabaseServices;
using StringMaster.Extensions;
using StringMaster.Services.Implementation;
using StringMaster.UI.Models;

namespace StringMaster.Helpers;

public static class LayerHelpers
{
    /// <summary>
    /// Check if the database contains the specified layer
    /// </summary>
    /// <param name="layerName">Name of the layer.</param>
    /// <param name="tr">The transaction.</param>
    /// <param name="database">The current database.</param>
    /// <returns><c>true</c> if the specified layer name has layer; otherwise, <c>false</c>.</returns>
    public static bool HasLayer(string layerName, Transaction tr, Database database)
    {
        if (string.IsNullOrEmpty(layerName))
            return false;

        var layerTable = tr.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;

        return layerTable is not null && layerTable.Has(layerName);
    }

    /// <summary>
    /// Creates a layer in the database
    /// </summary>
    /// <param name="layer">AcadLayer object.</param>
    /// <param name="tr">The transaction.</param>
    /// <param name="database">The current database.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void CreateLayer(AcadLayer layer, Transaction tr, Database database)
    {
        if (string.IsNullOrEmpty(layer.Name))
            throw new ArgumentNullException(nameof(layer.Name));

        var layerTable = (LayerTable)tr.GetObject(database.LayerTableId, OpenMode.ForRead);

        if (layerTable.Has(layer.Name))
            return;

        LinetypeTableRecord lineType = null;

        var ltTable = (LinetypeTable)tr.GetObject(AcadApplicationService.ActiveDatabase.LinetypeTableId, OpenMode.ForRead);
        if (ltTable.Has(layer.Linetype))
        {
            foreach (ObjectId objectId in ltTable)
            {
                lineType = (LinetypeTableRecord)tr.GetObject(objectId, OpenMode.ForRead);
                if (lineType.Name == layer.Linetype)
                    break;
            }
        }

        var ltr = new LayerTableRecord
        {
            Name = layer.Name,
            Color = layer.Color.ToColor(),
            IsLocked = layer.IsLocked,
            IsFrozen = layer.IsFrozen,
            IsOff = true, // HACK: always turn on new layer.
            LineWeight = LineweightHelpers.LineweightStringtoLineweight(layer.Lineweight)
        };

        if (lineType != null)
            ltr.LinetypeObjectId = lineType.ObjectId;

        layerTable.UpgradeOpen();
        layerTable.Add(ltr);
        tr.AddNewlyCreatedDBObject(ltr, true);
    }
}
