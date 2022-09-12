using System;
using StringMaster.Models;

namespace StringMaster.Helpers;

public static class PointHelpers
{
    /// <summary>
    /// Converts a <see cref="Angle" /> object and distance to a <see cref="Point" />.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="basePoint">The base point to calculate the new <see cref="Point" /> from.</param>
    /// <returns>
    /// A <see cref="Point" /> containing the coordinates generated from the <see cref="Angle" />
    /// and distance.
    /// </returns>
    public static Point AngleAndDistanceToPoint(Angle angle, double distance, Point basePoint)
    {
        double dec = angle.ToDecimalDegrees();
        double rad = MathHelpers.DecimalDegreesToRadians(dec); //Millionths

        double departure = Math.Round(Math.Sin(rad) * distance, 10);
        double latitude = Math.Round(Math.Cos(rad) * distance, 10);

        double newX = basePoint.X + departure;
        double newY = basePoint.Y + latitude;

        return new Point(newX, newY);
    }

    /// <summary>
    /// Calculates the third point in a rectangle given 3 existing points.
    /// </summary>
    /// <param name="point1">The base point.</param>
    /// <param name="point2">The second point.</param>
    /// <param name="point3">The third point.</param>
    /// <remarks>Elevation is calculated as an average of the 3 points.</remarks>
    /// <returns>A <see cref="Point" /> representing the third point of a rectangle.</returns>
    public static Point CalculateRectanglePoint(Point point1, Point point2, Point point3)
    {
        double distance = GetDistanceBetweenPoints(point2, point3);
        Angle angle = AngleHelpers.GetAngleBetweenPoints(point2, point3);

        Point newPoint = AngleAndDistanceToPoint(angle, distance, point1);
        return newPoint;
    }

    /// <summary>
    /// Calculates a return leg based on two <see cref="Point" />s.
    /// </summary>
    /// <param name="point1">The base point.</param>
    /// <param name="point2">The second point for bearing.</param>
    /// <param name="leftLeg">If true, calculated point is left of line, else right of line.</param>
    /// <param name="distance">Leg distance.</param>
    /// <returns>A <see cref="Point" /> representing the new points location.</returns>
    public static Point CalculateRightAngleTurn(Point point1, Point point2, bool leftLeg = true, double distance = 2.0)
    {
        Angle forwardAngle = AngleHelpers.GetAngleBetweenPoints(point1, point2);

        if (leftLeg)
            forwardAngle -= 90;
        else
            forwardAngle += 90;

        Point newPoint = AngleAndDistanceToPoint(forwardAngle, distance, point1);
        return newPoint;
    }

    /// <summary>
    /// Gets distance between two coordinates.
    /// </summary>
    /// <param name="point1">The first coordinate.</param>
    /// <param name="point2">The second coordinate.</param>
    /// <param name="useRounding"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns>A double representing the distance between the two coordinates.</returns>
    public static double GetDistanceBetweenPoints(Point point1, Point point2, bool useRounding = false, int decimalPlaces = 4)
    {
        return MathHelpers.GetDistanceBetweenPoints(point1.X, point2.X, point1.Y, point2.Y, useRounding, decimalPlaces);
    }
}
