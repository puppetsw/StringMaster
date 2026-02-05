using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using StringMaster.Civil.Extensions;
using StringMaster.Civil.Helpers;
using StringMaster.Common.Extensions;
using StringMaster.Common.Helpers;
using StringMaster.UI.Helpers;
using StringMaster.UI.Models;
using StringMaster.UI.Services.Interfaces;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace StringMaster.Civil;

public class StringCivilPointService : IStringCivilPointsService
{
	public void StringCivilPoints(IList<DescriptionKey> descriptionKeys, string filterByPointGroup = "")
	{
		var doc = Application.DocumentManager.MdiActiveDocument;
		var civilDoc = CivilApplication.ActiveDocument;

		using Transaction tr = doc.TransactionManager.StartLockedTransaction();
		var desMapping = new Dictionary<string, DescriptionKeyMatch>();

		foreach (ObjectId pointId in civilDoc.CogoPoints)
		{
			var cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;
			if (cogoPoint == null)
				continue;

			// Check if we need to filter by PointGroup
			if (!string.IsNullOrEmpty(filterByPointGroup))
			{
				var pointGroup = cogoPoint.PrimaryPointGroupId.GetObject(OpenMode.ForRead) as PointGroup;
				if (pointGroup != null && !pointGroup.Name.Equals(filterByPointGroup))
				{
					// Skip this point. It doesn't belong to the specified PointGroup 
					continue;
				}
			}

			foreach (DescriptionKey descriptionKey in descriptionKeys)
			{
				if (!DescriptionKeyMatch.IsMatch(cogoPoint.RawDescription, descriptionKey))
					continue;

				string description = DescriptionKeyMatch.Description(cogoPoint.RawDescription, descriptionKey);
				string lineNumber = DescriptionKeyMatch.LineNumber(cogoPoint.RawDescription, descriptionKey);
				string specialCode = DescriptionKeyMatch.SpecialCode(cogoPoint.RawDescription, descriptionKey);

				DescriptionKeyMatch deskeyMatch;
				if (desMapping.ContainsKey(description))
				{
					deskeyMatch = desMapping[description];
				}
				else
				{
					deskeyMatch = new DescriptionKeyMatch(descriptionKey);
					desMapping.Add(description, deskeyMatch);
				}

				deskeyMatch.AddPoint(cogoPoint.ToPoint(), lineNumber, specialCode);

			}
		}

		var bt = (BlockTable) tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead);
		var btr = (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

		foreach (var desKey in desMapping)
		{
			DescriptionKeyMatch deskeyMatch = desKey.Value;

			foreach (var surveyPoints in deskeyMatch.SurveyPoints)
			{
				var pointList = new List<SurveyPointList>();
				var points = new SurveyPointList();

				for (var i = 0; i < surveyPoints.Value.Count; i++)
				{
					var point = surveyPoints.Value[i];

					try
					{
						if (point.HasSpecialCode)
						{
							if (point.SpecialCode.Equals(".S") && points.Count != 0)
							{
								pointList.Add(points);
								points = new SurveyPointList();
							}

							switch (point.SpecialCode)
							{
								case ".SL":
								case ".L":
								{
									var point1 = point.Point;
									var point2 = surveyPoints.Value[i + 1].Point;
									var newPoint = PointHelpers.CalculateRightAngleTurn(point1, point2);

									var cloned = (SurveyPoint)point.Clone();
									cloned.Point.X = newPoint.X;
									cloned.Point.Y = newPoint.Y;
									cloned.Point.Z = newPoint.Z;

									points.Add(cloned);
									break;
								}
								case ".SR":
								case ".R":
								{
									var point1 = point.Point;
									var point2 = surveyPoints.Value[i + 1].Point;
									var newPoint = PointHelpers.CalculateRightAngleTurn(point1, point2, false);

									var cloned = (SurveyPoint)point.Clone();
									cloned.Point.X = newPoint.X;
									cloned.Point.Y = newPoint.Y;
									cloned.Point.Z = newPoint.Z;

									points.Add(cloned);
									break;
								}
								case ".RECT":
								{
									var point1 = point.Point;
									var point2 = surveyPoints.Value[i - 1].Point;
									var point3 = surveyPoints.Value[i - 2].Point;
									var newPoint = PointHelpers.CalculateRectanglePoint(point1, point2, point3);
									var averageZ = (point1.Z + point2.Z + point3.Z) / 3;

									var cloned = (SurveyPoint)point.Clone();
									cloned.Point.X = newPoint.X;
									cloned.Point.Y = newPoint.Y;
									cloned.Point.Z = averageZ;

									points.Add(point);
									points.Add(cloned);
									continue;
								}
								case ".CLS":
								{
									// Don't need to do anything here.
									// It's handled by the SurveyPoint constructor.
									break;
								}
							}
						}
						points.Add(point);
					}
					catch (IndexOutOfRangeException e)
					{
						// Console.WriteLine($"Coding error at: PT#{point.CogoPoint.PointNumber}, DES:{point.CogoPoint.RawDescription}");
						Console.WriteLine(e.Message);
					}
				}

				pointList.Add(points);

				string layerName = deskeyMatch.DescriptionKey.Layer;

				if (deskeyMatch.DescriptionKey.AcadLayer.IsSelected)
				{
					if (!LayerHelpers.HasLayer(layerName, tr, Application.DocumentManager.MdiActiveDocument.Database))
						LayerHelpers.CreateLayer(deskeyMatch.DescriptionKey.AcadLayer, tr, Application.DocumentManager.MdiActiveDocument.Database);
				}
				else
				{
					layerName = "0"; // use default layer if not selected.
				}

				foreach (var list in pointList)
				{
					bool hasCurve = false;
					bool isClosed = false;
					var pointCollection = new Point3dCollection();
					var midPointCollection = new Point3dCollection();
					var polyline = new Polyline();
					for (var index = 0; index < list.Count; index++)
					{
						var surveyPoint = list[index];

						if (surveyPoint.IsProcessed)
							continue;

						if (surveyPoint.Closed)
							isClosed = true;

						if (surveyPoint.StartCurve)
						{
							// check if the end curve point is 2 more points away.
							if (!list[index + 2].EndCurve)
							{
								surveyPoint.IsProcessed = true;
								pointCollection.Add(surveyPoint.Point.ToPoint3d());
								polyline.AddVertexAt(index, surveyPoint.Point.ToPoint2d(), 0, 0, 0);
								break;
							}

							hasCurve = true;

							surveyPoint.IsProcessed = true;

							var startPoint = surveyPoint.Point;
							var midPoint = list[index + 1].Point;
							var endPoint = list[index + 2].Point;

							midPointCollection.Add(midPoint.ToPoint3d());

							// Set the midpoint of arc to processed.
							// So that it doesn't get added again.
							list[index + 1].IsProcessed = true;

							var arc = new CircularArc2d(startPoint.ToPoint2d(), midPoint.ToPoint2d(), endPoint.ToPoint2d());
							var bulge = arc.GetArcBulge();

							pointCollection.Add(surveyPoint.Point.ToPoint3d());

							polyline.AddVertexAt(index, surveyPoint.Point.ToPoint2d(), bulge, 0, 0);
							index++;
							polyline.AddVertexAt(index, endPoint.ToPoint2d(), 0, 0, 0);
						}
						else
						{
							surveyPoint.IsProcessed = true;
							pointCollection.Add(surveyPoint.Point.ToPoint3d());
							polyline.AddVertexAt(index, surveyPoint.Point.ToPoint2d(), 0, 0, 0);
						}
					}

					if (pointCollection.Count <= 1)
						continue;

					// Draw the polylines.
					if (deskeyMatch.DescriptionKey.Draw2D && !hasCurve)
						PolylineHelpers.DrawPolyline2d(tr, btr, pointCollection, layerName, deskeyMatch.DescriptionKey.AcadColor.ToColor(), isClosed);

					if (deskeyMatch.DescriptionKey.Draw3D && !hasCurve)
						PolylineHelpers.DrawPolyline3d(tr, btr, pointCollection, layerName, deskeyMatch.DescriptionKey.AcadColor.ToColor(), isClosed);

				}
			}
		}
		tr.Commit();
	}
}