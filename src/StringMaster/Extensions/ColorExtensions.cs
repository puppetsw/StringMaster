﻿using Autodesk.AutoCAD.Colors;
using StringMaster.Models;

namespace StringMaster.Extensions;

public static class ColorExtensions
{
    public static Color ToColor(this AcadColor acadColor)
    {
        if (acadColor.ColorIndex is not null)
        {
            if (acadColor.IsByLayer)
                return Color.FromColorIndex(ColorMethod.ByLayer, 256);
            if (acadColor.IsByBlock)
                return Color.FromColorIndex(ColorMethod.ByBlock, (short)acadColor.ColorIndex);

            return Color.FromColorIndex(ColorMethod.ByAci, (short)acadColor.ColorIndex);
        }

        return Color.FromRgb(acadColor.Red, acadColor.Green, acadColor.Blue);
    }
}