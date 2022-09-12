using System;

namespace StringMaster.Models;

/// <summary>
/// A class for working with surveyors bearings in degrees, minutes, seconds.
/// Works to Australian standards.
/// </summary>
public sealed class Angle : IEquatable<Angle>
{
    /// <summary>
    /// Gets or sets the degrees.
    /// </summary>
    public int Degrees { get; set; }

    /// <summary>
    /// Gets or sets the minutes.
    /// </summary>
    public int Minutes { get; set; }

    /// <summary>
    /// Gets or sets the seconds.
    /// </summary>
    public int Seconds { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Angle"/> class.
    /// </summary>
    public Angle()
    {
        Degrees = 0;
        Minutes = 0;
        Seconds = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Angle" /> class.
    /// </summary>
    /// <param name="degrees">The degrees.</param>
    /// <param name="minutes">The minutes.</param>
    /// <param name="seconds">The seconds.</param>
    public Angle(int degrees, int minutes, int seconds)
    {
        Degrees = degrees;
        Minutes = minutes;
        Seconds = seconds;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Angle"/> class.
    /// </summary>
    /// <param name="bearing">
    /// The bearing in degrees, minutes and seconds in decimal format.
    /// </param>
    public Angle(double bearing)
    {
        Angle angle = Parse(bearing);

        Degrees = angle.Degrees;
        Minutes = angle.Minutes;
        Seconds = angle.Seconds;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Angle"/> class.
    /// </summary>
    /// <param name="bearing">The bearing in degrees, minutes and seconds in string format.</param>
    public Angle(string bearing)
    {
        Angle angle;
        try
        {
            var b = Convert.ToDouble(bearing);
            angle = Parse(b);
        }
        catch
        {
            angle = new Angle();
        }

        Degrees = angle.Degrees;
        Minutes = angle.Minutes;
        Seconds = angle.Seconds;
    }

    /// <summary>
    /// Converts a bearing of degrees, minutes, seconds in decimal format to a <see cref="Angle"/> object.
    /// </summary>
    /// <param name="bearing">bearing in degrees, minutes and seconds. 354.5020 (354 degrees, 50 minutes, 20 seconds)</param>
    /// <returns>A <see cref="Angle"/> object containing the parsed value.</returns>
    public static Angle Parse(double bearing)
    {
        var angle = new Angle();

        var degrees = (int) Math.Truncate(bearing);
        var deg1 = Math.Round(bearing - degrees, 4);

        var minutes = (int) Math.Round(deg1 * 100, 4);
        var min1 = Math.Round(bearing - degrees, 4);
        var min2 = Math.Round(min1 * 100, 2);
        var min3 = Math.Round(min2 - minutes, 2);

        var seconds = (int) (min3 * 100);

        angle.Degrees = degrees;
        angle.Minutes = minutes;
        angle.Seconds = seconds;

        return angle;
    }

    /// <summary>
    /// Converts the number representation of a bearing to a <see cref="Angle"/>.
    /// A return value indicates whether the operation succeeded.
    /// </summary>
    /// <param name="bearing">The bearing.</param>
    /// <param name="angle">The angle.</param>
    /// <returns><c>true</c> if <c>bearing</c> was converted successfully, <c>false</c> otherwise.</returns>
    public static bool TryParse(double? bearing, out Angle angle)
    {
        angle = null;

        if (bearing == null)
            return false;

        Angle tempAngle = Parse(bearing.Value);

        if (IsValid(tempAngle))
        {
            angle = tempAngle;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true if the <see cref="Angle"/> object contains a valid bearing.
    /// </summary>
    /// <param name="angle"><see cref="Angle"/> object to check.</param>
    /// <param name="limit">If limit is true then checks to see if Degrees is less than 360.</param>
    public static bool IsValid(Angle angle, bool limit = true)
    {
        if (limit)
            return angle.Degrees < 360 && angle.Minutes < 60 && angle.Seconds < 60;

        return angle.Minutes < 60 && angle.Seconds < 60;
    }

    /// <summary>
    /// Returns true if the <see cref="Angle"/> object contains a valid bearing.
    /// </summary>
    /// <param name="bearing">The bearing represented as a double value.</param>
    /// <param name="limit"></param>
    public static bool IsValid(double bearing, bool limit = true)
    {
        Angle angle = Parse(bearing);
        return IsValid(angle, limit);
    }

    public static Angle operator +(Angle angle1, Angle angle2) => Add(angle1, angle2, true);

    public static Angle operator +(Angle angle1, int degrees) => angle1 + new Angle(degrees);

    public static Angle operator -(Angle angle1, Angle angle2) => Subtract(angle1, angle2, true);

    public static Angle operator -(Angle angle1, int degrees) => angle1 - new Angle(degrees);

    ///<inheritdoc/>
    public override string ToString()
    {
        string mins, secs;

        if (Minutes < 10)
        {
            mins = "0" + Minutes; //add the 0 in front if its less than 10.
        }
        else
        {
            mins = Minutes.ToString();
        }

        if (Seconds < 10)
        {
            secs = "0" + Seconds; //add the 0 in front if its less than 10.
        }
        else
        {
            secs = Seconds.ToString();
        }

        return string.Format(Degrees + "°" + mins + "'" + secs + '"');
    }

    /// <summary>
    /// Returns a double value that represents the current object.
    /// </summary>
    /// <returns>A double value that represents the current object.</returns>
    public double ToDouble()
    {
        double bearing = Degrees + ((double) Minutes / 100) + ((double) Seconds / 10000);
        return bearing;
    }

    public static Angle Add(Angle angle1, Angle angle2, bool limit = false)
    {
        int degrees = angle1.Degrees + angle2.Degrees;
        int minutes = angle1.Minutes + angle2.Minutes;
        int seconds = angle1.Seconds + angle2.Seconds;

        // Work out seconds first, carry over to minutes.
        if (seconds >= 60)
        {
            seconds -= 60;
            minutes++;
        }

        // Work out minutes, carry over to degrees.
        if (minutes >= 60)
        {
            minutes -= 60;
            degrees++;
        }

        if (limit && degrees > 360)
        {
            degrees -= 360;
        }

        return new Angle { Degrees = degrees, Minutes = minutes, Seconds = seconds };
    }

    public static Angle Add(double bearing1, double bearing2, bool limit = false)
    {
        var angle1 = new Angle(bearing1);
        var angle2 = new Angle(bearing2);

        return Add(angle1, angle2, limit);
    }

    public static Angle Subtract(Angle angle1, Angle angle2, bool limit = false)
    {
        int degrees = angle1.Degrees - angle2.Degrees;
        int minutes = angle1.Minutes - angle2.Minutes;
        int seconds = angle1.Seconds - angle2.Seconds;

        // Work out seconds first, carry over to minutes.
        if (angle1.Seconds < angle2.Seconds)
        {
            minutes--;
            seconds += 60;
        }

        // Work out minutes, carry over to degrees.
        if (angle1.Minutes < angle2.Minutes)
        {
            degrees--;
            minutes += 60;
        }

        if (limit && degrees <= 0)
        {
            degrees += 360;
        }

        return new Angle { Degrees = degrees, Minutes = minutes, Seconds = seconds };
    }

    public static Angle Subtract(double bearing1, double bearing2, bool limit = false)
    {
        var angle1 = new Angle(bearing1);
        var angle2 = new Angle(bearing2);

        return Subtract(angle1, angle2, limit);
    }

    public bool IsZero => Degrees == 0 && Minutes == 0 && Seconds == 0;

    public bool Equals(Angle other)
    {
        if (other == null)
        {
            return false;
        }

        return (this.Degrees == other.Degrees)
               && (this.Minutes == other.Minutes)
               && (this.Seconds == other.Seconds);
    }

    public override bool Equals(object obj)
    {
        return obj is Angle angle && Equals(angle);
    }

    public override int GetHashCode()
    {
        return Degrees.GetHashCode() ^ Minutes.GetHashCode() ^ Seconds.GetHashCode();
    }
}
