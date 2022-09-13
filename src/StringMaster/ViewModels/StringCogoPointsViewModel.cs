using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using StringMaster.Extensions;
using StringMaster.Helpers;
using StringMaster.Models;
using StringMaster.Utilities;

namespace StringMaster.ViewModels;

public class StringCogoPointsViewModel : ObservableObject
{
    private ObservableCollection<DescriptionKey> _descriptionKeys;

    public ObservableCollection<DescriptionKey> DescriptionKeys
    {
        get => _descriptionKeys;
        set => SetProperty(ref _descriptionKeys, value);
    }

    public DescriptionKey SelectedKey { get; set; }

    public ICommand AddRowCommand => new RelayCommand(AddRow);

    public ICommand RemoveRowCommand => new RelayCommand(RemoveRow);

    public ICommand StringCommand => new RelayCommand(StringCogoPoints, () => DescriptionKeys is not null &&
                                                                              DescriptionKeys.Count > 0 &&
                                                                              DescriptionKeys.All(x => x.IsValid()));

    public StringCogoPointsViewModel() => LoadSettings(Properties.Settings.Default.DescriptionKeyFileName);

    private void AddRow()
    {
        DescriptionKeys ??= new();
        DescriptionKeys.Add(new DescriptionKey());
    }

    private void RemoveRow()
    {
        if (DescriptionKeys is null)
            return;

        if (SelectedKey != null)
            DescriptionKeys?.Remove(SelectedKey);
    }

    // TODO: This method requires a massive cleanup.
    // The below method current works, but it's very messy and not structured well.
    // The idea was to get something that was working with the 'special codes'.
    // The feature line/poly/3dpoly line system needs a bit of work also.
    private void StringCogoPoints()
    {
        if (DescriptionKeys is null)
            return;

        RemoveInvalidDescriptionKeys();

        if (DescriptionKeys.Count == 0)
            return;

        if (!string.IsNullOrEmpty(_fileName))
            Save();

        using Transaction tr = CivilApplication.StartLockedTransaction();
        var desMapping = new Dictionary<string, DescriptionKeyMatch>();

        foreach (ObjectId pointId in CivilApplication.ActiveCivilDocument.CogoPoints)
        {
            var cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;
            if (cogoPoint == null)
                continue;

            foreach (DescriptionKey descriptionKey in DescriptionKeys)
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

                deskeyMatch.AddCogoPoint(cogoPoint.ToPoint(), lineNumber, specialCode);
            }
        }

        var bt = (BlockTable) tr.GetObject(CivilApplication.ActiveDatabase.BlockTableId, OpenMode.ForRead);
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

                if (!LayerUtils.HasLayer(layerName, tr, CivilApplication.ActiveDatabase))
                    LayerUtils.CreateLayer(layerName, tr, CivilApplication.ActiveDatabase);

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

                    // Draw the polylines.
                    if (deskeyMatch.DescriptionKey.Draw2D && !hasCurve)
                        PolylineUtils.DrawPolyline2d(tr, btr, pointCollection, layerName, isClosed);

                    if (deskeyMatch.DescriptionKey.Draw3D && !hasCurve)
                        PolylineUtils.DrawPolyline3d(tr, btr, pointCollection, layerName, isClosed);

                    if (deskeyMatch.DescriptionKey.DrawFeatureLine && !hasCurve)
                    {
                        var polylineId = btr.AppendEntity(polyline);
                        tr.AddNewlyCreatedDBObject(polyline, true);
                        var flId = FeatureLine.Create(string.Empty, polylineId);
                        var featureLine = (FeatureLine)tr.GetObject(flId, OpenMode.ForWrite);
                        featureLine.Layer = layerName;
                        featureLine.DowngradeOpen();
                        polyline.Erase();
                    }

                    // Draw featureline if curve is found.
                    if ((deskeyMatch.DescriptionKey.Draw2D || deskeyMatch.DescriptionKey.Draw3D ||
                         deskeyMatch.DescriptionKey.DrawFeatureLine) && hasCurve)
                    {
                        var polylineId = btr.AppendEntity(polyline);
                        tr.AddNewlyCreatedDBObject(polyline, true);

                        var featureLineId = FeatureLine.Create(string.Empty, polylineId);
                        var featureLine = (FeatureLine)tr.GetObject(featureLineId, OpenMode.ForWrite);

                        // Set layer.
                        featureLine.Layer = layerName;

                        // Set elevations.
                        for (int i = 0; i < featureLine.GetPoints(FeatureLinePointType.AllPoints).Count; i++)
                            featureLine.SetPointElevation(i, pointCollection[i].Z);

                        // Add midpoint for curve.
                        for (int i = 0; i < midPointCollection.Count; i++)
                        {
                            // Get position of the featureline
                            var pointOnFeatureLine = featureLine.GetClosestPointTo(midPointCollection[i], false);
                            // Create a new point using the correct height and position.
                            var midPoint = new Point3d(pointOnFeatureLine.X, pointOnFeatureLine.Y, midPointCollection[i].Z);
                            featureLine.InsertElevationPoint(midPoint);
                        }

                        // Delete the temporary polyline if 2D is not selected.
                        // As we already use a polyline to create the FL.
                        if (deskeyMatch.DescriptionKey.Draw2D)
                            polyline.Layer = layerName;
                        else
                            polyline.Erase();

                        if (deskeyMatch.DescriptionKey.Draw3D)
                        {
                            if (!featureLine.ConvertToPolyline3d(tr, out var polyline3d, desKey.Value.DescriptionKey.MidOrdinate))
                            {
                                Console.WriteLine("Error converting feature line to Polyline.");
                                continue;
                            }

                            btr.AppendEntity(polyline3d);
                            tr.AddNewlyCreatedDBObject(polyline3d, true);
                        }

                        if (!deskeyMatch.DescriptionKey.DrawFeatureLine)
                            featureLine.Erase();
                    }
                }
            }
        }
        tr.Commit();
    }

    private void RemoveInvalidDescriptionKeys()
    {
        // Remove invalid keys
        foreach (var itemToRemove in DescriptionKeys.Where(x => !x.IsValid()).ToList())
            DescriptionKeys.Remove(itemToRemove);
    }

    private string _fileName;

    /// <summary>
    /// Get the last xml file loaded from settings
    /// </summary>
    /// <param name="fileName"></param>
    public bool LoadSettings(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            if (File.Exists(fileName))
            {
                _fileName = fileName;
                Save();
                DescriptionKeys = XmlHelper.ReadFromXmlFile<ObservableCollection<DescriptionKey>>(fileName);
                return true;
            }
        }

        DescriptionKeys = new ObservableCollection<DescriptionKey>();
        return false;
    }

    /// <summary>
    /// Save XML file
    /// </summary>
    /// <param name="fileName"></param>
    public bool SaveSettings(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || DescriptionKeys == null)
            return false;

        RemoveInvalidDescriptionKeys();

        if (DescriptionKeys.Count == 0)
            return false;

        _fileName = fileName;
        XmlHelper.WriteToXmlFile(fileName, DescriptionKeys);
        Save();
        return true;
    }

    private void Save()
    {
        Properties.Settings.Default.DescriptionKeyFileName = _fileName;
        Properties.Settings.Default.Save();
        CivilApplication.Editor.WriteMessage("\nStringMaster: Settings Saved.");
    }
}
