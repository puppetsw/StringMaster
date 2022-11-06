using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using StringMaster.UI.Models;
using StringMaster.UI.Services.Interfaces;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace StringMaster.Services.Implementation;

public class AcadApplicationService : IAcadApplicationService
{
    /// <summary>
    /// Gets the <see cref="DocumentManager"/>.
    /// </summary>
    public static DocumentCollection DocumentManager => Application.DocumentManager;

    /// <summary>
    /// Gets the active <see cref="Database"/> object.
    /// </summary>
    public static Database ActiveDatabase => DocumentManager.MdiActiveDocument.Database;

    /// <summary>
    /// Gets the active <see cref="Editor"/> object.
    /// </summary>
    public static Editor Editor => DocumentManager.MdiActiveDocument.Editor;

    public IEnumerable<AcadDocument> Documents
    {
        get
        {
            var list = new List<AcadDocument>();

            foreach (Document document in Application.DocumentManager)
                list.Add(new AcadDocument { Name = document.Name });

            return list;
        }
    }

    public AcadDocument ActiveDocument => new() { Name = Application.DocumentManager.MdiActiveDocument.Name };

    public void WriteMessage(string message) => Editor.WriteMessage(message);
}
