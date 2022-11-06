using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

#if CIVIL

using Autodesk.Civil.ApplicationServices;

#endif

using StringMaster.Extensions;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace StringMaster;

/// <summary>
/// Provides access to several "active" objects and helper methods
/// in the AutoCAD Civil 3D runtime environment.
/// Registers services for dependency injection.
/// </summary>
public static class AcadApplication
{
    /// <summary>
    /// Gets the <see cref="DocumentManager"/>.
    /// </summary>
    public static DocumentCollection DocumentManager => Application.DocumentManager;

    /// <summary>
    /// Gets the active <see cref="Document"/> object.
    /// </summary>
    public static Document ActiveDocument => DocumentManager.MdiActiveDocument;

    /// <summary>
    /// Gets the active <see cref="Database"/> object.
    /// </summary>
    public static Database ActiveDatabase => ActiveDocument.Database;

    /// <summary>
    /// Gets the active <see cref="Editor"/> object.
    /// </summary>
    public static Editor Editor => ActiveDocument.Editor;

#if CIVIL
    public static CivilDocument ActiveCivilDocument => Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;

    public static bool IsCivil3DLoaded() => SystemObjects.DynamicLinker.GetLoadedModules().Contains("AecBase.dbx".ToLower());
#endif


    /// <summary>
    /// Starts a transaction.
    /// </summary>
    /// <returns>Transaction.</returns>
    public static Transaction StartTransaction()
    {
        return ActiveDocument.TransactionManager.StartTransaction();
    }

    /// <summary>
    /// Starts a locked transaction.
    /// </summary>
    /// <returns>Transaction.</returns>
    public static Transaction StartLockedTransaction()
    {
        return ActiveDocument.TransactionManager.StartLockedTransaction();
    }




}

