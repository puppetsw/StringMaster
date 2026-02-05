using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using StringMaster.Common.Extensions;
using StringMaster.UI.Services.Interfaces;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace StringMaster.Common.Services.Implementation
{
	public class CivilPointGroupService : ICivilPointGroupService
	{
		public IEnumerable<string> GetPointGroups()
		{
			var doc = Application.DocumentManager.MdiActiveDocument;
			var civilDoc = CivilApplication.ActiveDocument;

				using var tr = doc.TransactionManager.StartLockedTransaction();
				foreach (ObjectId pointGroupId in CivilApplication.ActiveDocument.PointGroups)
				{
					var pointGroup = tr.GetObject(pointGroupId, OpenMode.ForRead) as PointGroup;

					if (pointGroup != null)
					{
						yield return pointGroup.Name;
					}
				}
				tr?.Commit();
		}
	}
}
