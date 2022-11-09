using Autodesk.Civil.DatabaseServices;
using Point = StringMaster.UI.Models.Point;

namespace StringMaster.Civil.Extensions;

public static class CogoPointExtensions
{
    public static Point ToPoint(this CogoPoint cogoPoint) => new(cogoPoint.Easting, cogoPoint.Northing, cogoPoint.Elevation);
}