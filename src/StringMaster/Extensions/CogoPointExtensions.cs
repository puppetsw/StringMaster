using Autodesk.Civil.DatabaseServices;
using Point = StringMaster.Models.Point;

namespace StringMaster.Extensions;

public static class CogoPointExtensions
{
    public static Point ToPoint(this CogoPoint cogoPoint) => new(cogoPoint.Easting, cogoPoint.Northing, cogoPoint.Elevation);
}