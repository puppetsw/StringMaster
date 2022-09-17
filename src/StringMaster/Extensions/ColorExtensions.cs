using Autodesk.AutoCAD.Colors;
using StringMaster.Models;

namespace StringMaster.Extensions;

public static class ColorExtensions
{
    public static Color ToColor(this AcadColor acadColor)
    {
        if (acadColor.IsByLayer)
            return Color.FromColorIndex(ColorMethod.ByLayer, 256);

        if (acadColor.IsByBlock)
            return Color.FromColorIndex(ColorMethod.ByBlock, 0);

        if (acadColor.ColorIndex is not null)
            return Color.FromColorIndex(ColorMethod.ByAci, (short)acadColor.ColorIndex);

        return Color.FromRgb(acadColor.Red, acadColor.Green, acadColor.Blue);
    }
}
