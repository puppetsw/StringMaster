using Autodesk.AutoCAD.Colors;
using StringMaster.UI.Models;

namespace StringMaster.Common.Extensions;

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

    public static AcadColor ToAcadColor(this Color color)
    {
        if (color.IsByLayer)
            return AcadColor.ByLayer;

        if (color.IsByBlock)
            return AcadColor.ByBlock;

        return new AcadColor(color.ColorValue.R, color.ColorValue.G, color.ColorValue.B, color.ColorIndex);
    }
}
