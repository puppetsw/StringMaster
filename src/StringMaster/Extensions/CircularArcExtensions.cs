using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;

namespace StringMaster.Extensions;

public static class CircularArcExtensions
{
    private const double TOLERANCE = 1E-08;

    public static double GetArcBulge(this CircularArc2d circularArc)
    {
        double bulge = circularArc.EndAngle - circularArc.StartAngle;

        if (bulge < 0.0)
            bulge += 2.0 * Math.PI;

        if (circularArc.IsClockWise)
            bulge *= -1.0;

        return Math.Tan(bulge * 0.25);
    }

    public static double GetLength3D(this Curve3d circularArc) => circularArc.GetLength(0, 1, TOLERANCE);

    /// <summary>
    /// Gets the points that make up an arc at a specified mid-ordinate distance.
    /// </summary>
    /// <param name="circularArc">The <see cref="CircularArc3d"/> object.</param>
    /// <param name="midOrdinate">The mid-ordinate distance for point precision/amount.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Point3d"/> containing points along the arc.</returns>
    /// <remarks>Does not include the start of end points of the arc.</remarks>
    public static IEnumerable<Point3d> CurvePoints(this CircularArc3d circularArc, double midOrdinate = 0.01)
    {
        var point3dList = new List<Point3d>();
        double midOrdCalc = circularArc.Radius * 2.0 * Math.Acos((circularArc.Radius - midOrdinate) / circularArc.Radius);
        double stepDistance = circularArc.EndAngle / Math.Truncate(circularArc.GetLength3D() / midOrdCalc);
        double endAngle = circularArc.EndAngle;
        double distOnArc = stepDistance;

        while (distOnArc <= endAngle)
        {
            var point3d = circularArc.EvaluatePoint(distOnArc);
            point3dList.Add(point3d);
            distOnArc += stepDistance;
        }

        return point3dList;
    }

    public static double ArcLengthByMidOrdinate(double radius, double midOrdinate) =>
        2.0 * Math.Acos((radius - midOrdinate) / radius) * radius;
}
