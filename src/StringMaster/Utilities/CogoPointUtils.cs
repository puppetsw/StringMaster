using Autodesk.Civil.DatabaseServices;
using Point = StringMaster.Models.Point;

namespace StringMaster.Utilities;

public static class CogoPointUtils
{
    public static Point ToPoint(this CogoPoint cogoPoint) =>
        new(cogoPoint.Easting, cogoPoint.Northing, cogoPoint.Elevation);
}