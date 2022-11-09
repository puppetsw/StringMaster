using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using TransactionManager = Autodesk.AutoCAD.ApplicationServices.TransactionManager;

namespace StringMaster.Common.Extensions;

public static class TransactionExtensions
{
    /// <summary>
    /// Creates a new database transaction that allows to open,
    /// read, and modified objects in the database.
    /// </summary>
    /// <remarks>
    /// For use with Palettes as they require the document to be locked.
    /// </remarks>
    /// <returns>Returns a new database transaction and also locks the document.</returns>
    public static LockedTransaction StartLockedTransaction(this TransactionManager tm)
    {
        DocumentLock doclock = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.LockDocument();
        return new LockedTransaction(tm.StartTransaction(), doclock);
    }

    /// <summary>
    /// Creates a new database transaction that allows to open,
    /// read, and modified objects in the database.
    /// </summary>
    /// <remarks>
    /// For use with Palettes as they require the document to be locked.
    /// </remarks>
    /// <returns>Returns a new database transaction and also locks the document.</returns>
    public static LockedTransaction StartLockedTransaction(this TransactionManager tm, DocumentLockMode lockMode, string globalCommandName, string localCommandName, bool promptIfFails)
    {
        DocumentLock doclock = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.LockDocument(lockMode, globalCommandName, localCommandName, promptIfFails);
        return new LockedTransaction(tm.StartTransaction(), doclock);
    }
}

public sealed class LockedTransaction : Transaction
{
    private readonly DocumentLock _docLock;
    public LockedTransaction(Transaction tr, DocumentLock docLock) : base(tr.UnmanagedObject, tr.AutoDelete)
    {
        Interop.DetachUnmanagedObject(tr);
        GC.SuppressFinalize(tr);
        _docLock = docLock;
    }

    protected override void Dispose(bool A_0)
    {
        base.Dispose(A_0);
        if (A_0)
        {
            _docLock.Dispose();
        }
    }
}