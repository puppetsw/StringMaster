using System;
using System.Collections.Generic;

namespace StringMaster.UI.Models;

public sealed class SurveyPointList : List<SurveyPoint> { }

public sealed class SurveyPoint : IEquatable<SurveyPoint>, ICloneable
{
    public Point Point { get; set; }

    public bool HasSpecialCode => !string.IsNullOrEmpty(SpecialCode);

    public string SpecialCode { get; }

    public bool StartCurve { get; }

    public bool EndCurve { get; }

    public bool Closed { get; }

    public bool IsProcessed { get; set; }

    public SurveyPoint(Point point, string specialCode)
    {
        Point = point;
        SpecialCode = specialCode;

        switch (specialCode)
        {
            case ".SC":
            {
                StartCurve = true;
                break;
            }
            case ".EC":
            {
                EndCurve = true;
                break;
            }
            case ".RECT":
            {
                Closed = true;
                break;
            }
            case ".CLS":
            {
                Closed = true;
                break;
            }
        }
    }

    public bool Equals(SurveyPoint other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Equals(Point, other.Point) &&
               SpecialCode == other.SpecialCode &&
               StartCurve == other.StartCurve &&
               EndCurve == other.EndCurve;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((SurveyPoint)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Point.GetHashCode();
            hashCode = (hashCode * 397) ^ (SpecialCode != null ? SpecialCode.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ StartCurve.GetHashCode();
            hashCode = (hashCode * 397) ^ EndCurve.GetHashCode();
            hashCode = (hashCode * 397) ^ Closed.GetHashCode();
            return hashCode;
        }
    }

    public object Clone()
    {
        var surveyPoint = (SurveyPoint)MemberwiseClone();
        surveyPoint.Point = (Point)Point.Clone();
        return surveyPoint;
    }
}
