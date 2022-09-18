using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using StringMaster.Extensions;
using StringMaster.Models;
using StringMaster.Services.Interfaces;

namespace StringMaster.Services.Implementation;

public class AcadLayerService : IAcadLayerService
{
    public ObservableCollection<AcadLayer> Layers { get; } = new();

    public AcadLayerService()
    {
        GetLayers();
        Layers = new ObservableCollection<AcadLayer>(Layers.OrderBy(x => x.Name));
    }

    private void GetLayers()
    {
        using var tr = CivilApplication.StartLockedTransaction();

        var layerTable = (LayerTable)tr.GetObject(CivilApplication.ActiveDatabase.LayerTableId, OpenMode.ForRead);

        foreach (var objectId in layerTable)
        {
            var layer = (LayerTableRecord)tr.GetObject(objectId, OpenMode.ForRead);
            var color = layer.Color.ToAcadColor();

            Layers.Add(new AcadLayer(layer.Name, layer.IsOff, layer.IsFrozen, layer.IsLocked, color));
        }

        tr.Commit();
    }
}
