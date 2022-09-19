﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
using StringMaster.Services.Interfaces;
using StringMaster.Utilities;

// ReSharper disable UnusedMember.Global

namespace StringMaster.ViewModels;

public class StringCogoPointsViewModel : ObservableObject
{
    private bool _isUnsavedChanges;
    private readonly IOpenDialogService _openDialogService;
    private readonly ISaveDialogService _saveDialogService;
    private readonly IMessageBoxService _messageBoxService;
    private ObservableCollection<DescriptionKey> _descriptionKeys;
    private ObservableCollection<DescriptionKey> _unchangedDescriptionKeys;

    private string _currentFileName;

    public string CurrentFileName
    {
        get => _currentFileName;
        set => SetProperty(ref _currentFileName, value);
    }

    public bool IsUnsavedChanges
    {
        get => _isUnsavedChanges;
        set => SetProperty(ref _isUnsavedChanges, value);
    }

    public ObservableCollection<DescriptionKey> DescriptionKeys
    {
        get => _descriptionKeys;
        set => SetProperty(ref _descriptionKeys, value);
    }

    public DescriptionKey SelectedKey { get; set; }

    public ICommand NewDescriptionKeyFileCommand => new RelayCommand(NewDescriptionKeyFile);
    public ICommand OpenDescriptionKeyFileCommand => new RelayCommand(OpenDescriptionKeyFile);
    public ICommand SaveDescriptionKeyFileCommand => new RelayCommand(SaveDescriptionKeyFile, () => IsUnsavedChanges);
    public ICommand SaveAsDescriptionKeyFileCommand => new RelayCommand(SaveAsDescriptionKeyFile);
    public ICommand AddRowCommand => new RelayCommand(AddRow);
    public ICommand RemoveRowCommand => new RelayCommand(RemoveRow);
    public ICommand StringCommand => new RelayCommand(StringCogoPoints, () => DescriptionKeys is not null &&
                                                                              DescriptionKeys.Count > 0 &&
                                                                              DescriptionKeys.All(x => x.IsValid()));

    public StringCogoPointsViewModel(IOpenDialogService openDialogService,
                                     ISaveDialogService saveDialogService,
                                     IMessageBoxService messageBoxService)
    {
        _openDialogService = openDialogService;
        _saveDialogService = saveDialogService;
        _messageBoxService = messageBoxService;

        _openDialogService.DefaultExt = ".xml";
        _openDialogService.Filter = "XML Files (*.xml)|*.xml";

        _saveDialogService.DefaultExt = ".xml";
        _saveDialogService.Filter = "XML Files (*.xml)|*.xml";

        DescriptionKeys = new ObservableCollection<DescriptionKey>();

        LoadSettingsFromFile(Properties.Settings.Default.DescriptionKeyFileName);
    }

    /// <summary>
    /// Hook-up PropertyChanged events for sub-models.
    /// </summary>
    private void DescriptionKeysOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (DescriptionKey descriptionKey in e.NewItems)
            {
                descriptionKey.PropertyChanged += DescriptionKeyPropertyChanged;
                descriptionKey.AcadColor.PropertyChanged += DescriptionKeyPropertyChanged;
                descriptionKey.AcadLayer.PropertyChanged += DescriptionKeyPropertyChanged;
            }

        if (e.OldItems != null)
            foreach (DescriptionKey descriptionKey in e.OldItems)
            {
                descriptionKey.PropertyChanged -= DescriptionKeyPropertyChanged;
                descriptionKey.AcadColor.PropertyChanged -= DescriptionKeyPropertyChanged;
                descriptionKey.AcadLayer.PropertyChanged -= DescriptionKeyPropertyChanged;
            }
    }

    private void DescriptionKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        IsUnsavedChanges = !DescriptionKeys.SequenceEqual(_unchangedDescriptionKeys);
        NotifyPropertyChanged(nameof(IsUnsavedChanges));
    }

    private void AddRow()
    {
        DescriptionKeys ??= new();
        DescriptionKeys.Add(new DescriptionKey());
        IsUnsavedChanges = true;
        DescriptionKeyPropertyChanged(null, null);
    }

    private void RemoveRow()
    {
        if (DescriptionKeys is null)
            return;

        if (SelectedKey != null)
        {
            if (SelectedKey.IsValid())
            {
                var dialog = _messageBoxService.ShowYesNo("Delete", "Remove this description key? This cannot be undone.");
                if (dialog == true)
                    DescriptionKeys?.Remove(SelectedKey);
            }
            else
            {
                DescriptionKeys?.Remove(SelectedKey);
            }
        }

        IsUnsavedChanges = true;
        DescriptionKeyPropertyChanged(null, null);
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
                        PolylineUtils.DrawPolyline2d(tr, btr, pointCollection, layerName, deskeyMatch.DescriptionKey.AcadColor.ToColor(), isClosed);

                    if (deskeyMatch.DescriptionKey.Draw3D && !hasCurve)
                        PolylineUtils.DrawPolyline3d(tr, btr, pointCollection, layerName, deskeyMatch.DescriptionKey.AcadColor.ToColor(), isClosed);

                    if (deskeyMatch.DescriptionKey.DrawFeatureLine && !hasCurve)
                    {
                        var polylineId = btr.AppendEntity(polyline);
                        tr.AddNewlyCreatedDBObject(polyline, true);
                        var flId = FeatureLine.Create(string.Empty, polylineId);
                        var featureLine = (FeatureLine)tr.GetObject(flId, OpenMode.ForWrite);

                        // Set properties
                        featureLine.Layer = layerName;
                        featureLine.Color = deskeyMatch.DescriptionKey.AcadColor.ToColor();

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
                        featureLine.Color = deskeyMatch.DescriptionKey.AcadColor.ToColor();

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

    private void UnhookPropertyChangeEvents()
    {
        foreach (DescriptionKey descriptionKey in DescriptionKeys)
        {
            descriptionKey.PropertyChanged -= DescriptionKeyPropertyChanged;
            descriptionKey.AcadColor.PropertyChanged -= DescriptionKeyPropertyChanged;
        }
    }

    /// <summary>
    /// Get the last xml file loaded from settings
    /// </summary>
    /// <param name="fileName"></param>
    private void LoadSettingsFromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            return;

        DescriptionKeys.CollectionChanged -= DescriptionKeysOnCollectionChanged;
        UnhookPropertyChangeEvents();

        CurrentFileName = fileName;

        DescriptionKeys = new ObservableCollection<DescriptionKey>();
        DescriptionKeys.CollectionChanged += DescriptionKeysOnCollectionChanged;

        ObservableCollection<DescriptionKey> keysFromFile = null;
        try
        {
            keysFromFile = XmlHelper.ReadFromXmlFile<ObservableCollection<DescriptionKey>>(fileName);
        }
        catch (Exception e)
        {
            CivilApplication.Editor.WriteMessage("\nUnable to load description key file. ");
            CivilApplication.Editor.WriteMessage($"\n{e.Message}");
            Console.WriteLine(e);
            // Clone didn't work so we set it to empty
            _unchangedDescriptionKeys = new();
        }

        if (keysFromFile is not null)
        {
            SetUnchangedDescriptionKeys(keysFromFile);

            foreach (DescriptionKey key in keysFromFile)
                DescriptionKeys.Add(key);
        }

        IsUnsavedChanges = false;
        Properties.Settings.Default.DescriptionKeyFileName = fileName;
        Properties.Settings.Default.Save();
    }

    private void SetUnchangedDescriptionKeys(ObservableCollection<DescriptionKey> keysFromFile)
    {
        _unchangedDescriptionKeys = new ObservableCollection<DescriptionKey>();

        foreach (var item in keysFromFile)
        {
            if (item is ICloneable cloneable)
                _unchangedDescriptionKeys.Add((DescriptionKey)cloneable.Clone());
            else
                _unchangedDescriptionKeys.Add(item);
        }
    }

    /// <summary>
    /// Save XML file
    /// </summary>
    /// <param name="fileName"></param>
    private void SaveToFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || DescriptionKeys == null)
            throw new ArgumentNullException(nameof(fileName));

        RemoveInvalidDescriptionKeys();

        if (DescriptionKeys.Count == 0)
        {
            _messageBoxService.ShowWarning("StringMaster", "Unable to save file. No valid description keys found.");
            return;
        }

        XmlHelper.WriteToXmlFile(fileName, DescriptionKeys);
        SetUnchangedDescriptionKeys(DescriptionKeys);
        Properties.Settings.Default.DescriptionKeyFileName = fileName;
        Properties.Settings.Default.Save();
        IsUnsavedChanges = false;
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(CurrentFileName))
            SaveAs();
        else
            SaveToFile(CurrentFileName);
    }

    private void SaveAs()
    {
        var result = _saveDialogService.ShowDialog();
        if (result != true)
            return;

        CurrentFileName = _saveDialogService.FileName;
        SaveToFile(_saveDialogService.FileName);
    }

    private bool? CheckForUnsavedChangesAndContinue()
    {
        if (IsUnsavedChanges)
            return _messageBoxService.ShowYesNoCancel(StringHelpers.GetLocalizedString("UnsavedChangesTitle"),
                StringHelpers.GetLocalizedString("UnsavedChangesText"));
        return true;
    }

    // View Commands
    private void SaveDescriptionKeyFile() => Save();

    private void SaveAsDescriptionKeyFile() => SaveAs();

    private void NewDescriptionKeyFile()
    {
        var continueWithChanges = CheckForUnsavedChangesAndContinue();
        switch (continueWithChanges)
        {
            case true: // Do discard, or we can continue
                break;
            case false: // Don't discard changes
                Save();
                break;
            case null: // Cancelled
                return;
        }

        CurrentFileName = string.Empty;
        IsUnsavedChanges = true;
        UnhookPropertyChangeEvents();
        DescriptionKeys = new ObservableCollection<DescriptionKey>();
        DescriptionKeys.CollectionChanged += DescriptionKeysOnCollectionChanged;
        _unchangedDescriptionKeys = new ObservableCollection<DescriptionKey>();
    }

    private void OpenDescriptionKeyFile()
    {
        var continueWithChanges = CheckForUnsavedChangesAndContinue();
        switch (continueWithChanges)
        {
            case true: // Do discard, or we can continue
                break;
            case false: // Don't discard changes
                Save();
                break;
            case null:  // Cancelled
                return;
        }

        var dialog = _openDialogService.ShowDialog();
        if (dialog != true)
            return;

        CurrentFileName = _openDialogService.FileName;
        LoadSettingsFromFile(CurrentFileName);
    }
}
