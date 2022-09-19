using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using StringMaster.Extensions;
using StringMaster.Models;
using StringMaster.Services.Interfaces;

namespace StringMaster.Services.Implementation;

public class AcadLayerService : IAcadLayerService
{
    public ObservableCollection<AcadLayer> Layers { get; private set; }

    public AcadLayerService()
    {
        GetLayers();
    }

    private void GetLayers()
    {
        var layerList = new List<AcadLayer>();

        using var tr = CivilApplication.StartLockedTransaction();

        var layerTable = (LayerTable)tr.GetObject(CivilApplication.ActiveDatabase.LayerTableId, OpenMode.ForRead);

        foreach (var objectId in layerTable)
        {
            var layer = (LayerTableRecord)tr.GetObject(objectId, OpenMode.ForRead);
            var color = layer.Color.ToAcadColor();

            layerList.Add(new AcadLayer(layer.Name, layer.IsOff, layer.IsFrozen, layer.IsLocked, color));
        }

        tr.Commit();

        Layers = new ObservableCollection<AcadLayer>(layerList.OrderBy(x => x.Name));
    }
}
