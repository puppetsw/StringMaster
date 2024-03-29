﻿using System;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using StringMaster.UI.Models;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace StringMaster.Common.Helpers;

public static class PolylineHelpers
{
    public static ObjectId DrawPolyline3d(Transaction tr, BlockTableRecord btr, Point3dCollection points, string layerName, Color color, bool closed = false)
    {
        using var pLine3d = new Polyline3d(Poly3dType.SimplePoly, points, closed)
        {
            Layer = layerName,
            Color = color
        };

        ObjectId id = btr.AppendEntity(pLine3d);
        tr.AddNewlyCreatedDBObject(pLine3d, true);
        return id;
    }

    public static ObjectId DrawPolyline2d(Transaction tr, BlockTableRecord btr, Point3dCollection points, string layerName, Color color, bool closed = false)
    {
        using var pLine2d = new Polyline2d(Poly2dType.SimplePoly, points, 0, closed, 0, 0, null);
        using var pLine = new Polyline();
        pLine.ConvertFrom(pLine2d, false);
        pLine.Layer = layerName;
        pLine.Color = color;
        pLine.Elevation = 0;
        ObjectId id = btr.AppendEntity(pLine);
        tr.AddNewlyCreatedDBObject(pLine, true);

        return id;
    }

    public static RadiusPoint SegmentRadiusPoint(this Polyline polyline, double param)
    {
        var radiusPoint = new RadiusPoint();
        double bulgeAt = polyline.GetBulgeAt((int) param);

        if (Math.Abs(bulgeAt) > 0.0)
        {
            var secondDerivative = polyline.GetSecondDerivative(param);

            int bulgeDirection;
            if (bulgeAt > 0.0)
            {
                bulgeDirection = -1;
            }
            else
            {
                bulgeDirection = 1;
            }

            radiusPoint.Radius = secondDerivative.Length * bulgeDirection;

            var num2 = secondDerivative.AngleOnPlane(new Plane(new Point3d(0.0, 0.0, 0.0), Vector3d.ZAxis));
            var pointAtParameter = polyline.GetPointAtParameter(param);

            radiusPoint.Point = new Point(
                pointAtParameter.X - Math.Cos(num2) * radiusPoint.Radius,
                pointAtParameter.Y - Math.Sin(num2) * radiusPoint.Radius, 0.0);
        }

        return radiusPoint;
    }
}
