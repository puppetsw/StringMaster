using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows;
using StringMaster.Extensions;
using StringMaster.UI.Services.Interfaces;

namespace StringMaster.Services.Implementation;

public class AcadLinetypeDialogService : IAcadLinetypeDialogService
{
    public string ShowDialog(string linetype = null)
    {
        var ltd = new LinetypeDialog();

        using var tr = AcadApplicationService.DocumentManager.MdiActiveDocument.TransactionManager.StartLockedTransaction();

        var ltid = (LinetypeTable)tr.GetObject(AcadApplicationService.ActiveDatabase.LinetypeTableId, OpenMode.ForRead);
        if (!ltid.Has(linetype))
        {
            foreach (ObjectId objectId in ltid)
            {
                var ltr = (LinetypeTableRecord)tr.GetObject(objectId, OpenMode.ForRead);
                if (ltr.Name == linetype)
                {
                    ltd.Linetype = ltr.ObjectId;
                    break;
                }
            }
        }


        var dr = ltd.ShowDialog();


        var lineType = (LinetypeTableRecord)tr.GetObject(ltd.Linetype, OpenMode.ForRead);
        string layerName = lineType.Name;

        tr.Commit();

        if (dr == DialogResult.OK)
            return layerName;
        if (string.IsNullOrEmpty(linetype))
            return "Continuous";
        return linetype;
    }
}
