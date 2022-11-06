using System;
using StringMaster.UI.Models;

namespace StringMaster.UI.Helpers;

public static class AngleHelpers
{
    private const double TOLERANCE = 0.000000001;

    /// <summary>
    /// Converts a decimal degrees value to <see cref="Angle" /> object.
    /// </summary>
    /// <param name="decimalDegrees"></param>
    /// <returns>A <see cref="Angle" /> representing the converted decimal degrees values.</returns>
    public static Angle DecimalDegreesToAngle(double decimalDegrees)
    {
        double degrees = Math.Floor(decimalDegrees);
        double minutes = Math.Floor((decimalDegrees - degrees) * 60);
        double seconds = Math.Round(((decimalDegrees - degrees) * 60 - minutes) * 60, 0);

        if (Math.Abs(seconds - 60) < TOLERANCE)
        {
            minutes++;
            seconds = 0;
        }

        if (Math.Abs(minutes - 60) < TOLERANCE)
        {
            degrees++;
            minutes = 0;
        }

        return new Angle { Degrees = (int)degrees, Minutes = (int)minutes, Seconds = (int)seconds };
    }

    /// <summary>
    ///  Gets angle/bearing between two coordinates
    /// </summary>
    /// <param name="x1">Easting of first coordinate</param>
    /// <param name="x2">Easting of second coordinate</param>
    /// <param name="y1">Northing of first coordinate</param>
    /// <param name="y2">Northing of second coordinate</param>
    /// <returns>A <see cref="Angle" /> representing the angle/bearing between the two coordinates.</returns>
    public static Angle GetAngleBetweenPoints(double x1, double x2, double y1, double y2)
    {
        double rad = Math.Atan2(x2 - x1, y2 - y1);

        if (rad < 0)
            rad += 2 * Math.PI; // If radians is less than 0 add 2PI.

        double decDeg = Math.Abs(rad) * 180 / Math.PI;
        return DecimalDegreesToAngle(decDeg);
    }

    /// <summary>
    /// Flips an <see cref="Angle"/> 180°.
    /// </summary>
    /// <param name="angle">The angle to be flipped.</param>
    /// <returns>A <see cref="Angle"/> containing the flipped values.</returns>
    public static Angle Flip(this Angle angle)
    {
        return angle - new Angle(180);
    }

    /// <summary>
    /// Gets angle/bearing between two coordinates
    /// </summary>
    /// <param name="point1">The first coordinate.</param>
    /// <param name="point2">The second coordinate.</param>
    /// <returns>A <see cref="Angle" /> representing the angle/bearing between the two coordinates.</returns>
    public static Angle GetAngleBetweenPoints(Point point1, Point point2)
    {
        return AngleHelpers.GetAngleBetweenPoints(point1.X, point2.X, point1.Y, point2.Y);
    }

    /// <summary>
    /// Converts a radians value to <see cref="Angle"/> object.
    /// </summary>
    /// <param name="radians">The radians.</param>
    /// <returns>A <see cref="Angle"/> representing the converted radians value.</returns>
    public static Angle RadiansToAngle(double radians)
    {
        var decimalDegrees = MathHelpers.RadiansToDecimalDegrees(radians);
        return DecimalDegreesToAngle(decimalDegrees);
    }

    /// <summary>
    /// Converts a <see cref="Angle" /> to decimal degrees.
    /// </summary>
    /// <param name="angle">The <see cref="Angle" /> to convert.</param>
    /// <returns>A double representing the <see cref="Angle" /> in decimal degrees.</returns>
    /// <remarks>The returned value is rounded to 4 decimal places, unless otherwise specified.</remarks>
    public static double ToDecimalDegrees(this Angle angle)
    {
        if (angle == null)
            return 0;

        double minutes = (double)angle.Minutes / 60;
        double seconds = (double)angle.Seconds / 3600;

        return angle.Degrees + minutes + seconds;
    }

    /// <summary>
    /// Converts a <see cref="Angle"/> to radians.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <returns>A double representing the <see cref="Angle"/> in radians.</returns>
    public static double ToRadians(this Angle angle)
    {
        return MathHelpers.DecimalDegreesToRadians(angle.ToDecimalDegrees());
    }
}
