#nullable enable

using System;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Windows;
using StringMaster.UI.Models;
using StringMaster.UI.Services.Interfaces;

namespace StringMaster.Common.Services.Implementation;

public class AcadColorDialogService : IAcadColorDialogService
{
    public AcadColor? ShowDialog()
    {
        var dialog = new ColorDialog();

        if (dialog.ShowModal() != true)
            return null;

        var selectedColor = dialog.Color;

        if (selectedColor.IsByAci)
        {
            if (selectedColor.IsByLayer)
                return AcadColor.ByLayer;

            if (selectedColor.IsByBlock)
                return AcadColor.ByBlock;

            AcadColor acadColor = new(selectedColor.ColorValue.R, selectedColor.ColorValue.G, selectedColor.ColorValue.B, selectedColor.ColorIndex);

            return acadColor;
        }
        else
        {
            var colorIndex = selectedColor.ColorIndex;
            var colorByte = Convert.ToByte(colorIndex);
            var rgb = EntityColor.LookUpRgb(colorByte);
            var b = rgb & 0xffL;
            var g = rgb & 0xff00L >> 8;
            var r = rgb >> 16;

            AcadColor acadColor = new((byte)r, (byte)g, (byte)b, colorIndex);

            return acadColor;
        }
    }
}
