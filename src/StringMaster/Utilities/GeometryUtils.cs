using Autodesk.AutoCAD.Geometry;

namespace StringMaster.Utilities;

public static class GeometryUtils
{
    public static Plane PlaneXY { get; } = new(new Point3d(0.0, 0.0, 0.0), Vector3d.ZAxis);
}