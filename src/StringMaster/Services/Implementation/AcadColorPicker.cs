using System;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Windows;
using StringMaster.Models;
using StringMaster.Services.Interfaces;

namespace StringMaster.Services.Implementation;

/// <summary>
/// AutoCAD ColorPicker dialog implementation.
/// Inspired by the control created by Balaji Ramamoorthy
/// https://adndevblog.typepad.com/autocad/2013/07/wpf-implementation-to-mimic-color-layer-controls.html
/// </summary>
public class AcadColorPicker : IAcadColorPicker
{
    public AcadColors Colors { get; } = new();

    public AcadColor GetAcadColor()
    {
        var dialog = new ColorDialog();

        if (dialog.ShowModal() != true)
            return null;

        var selectedColor = dialog.Color;

        if (selectedColor.IsByAci)
        {
            if (selectedColor.IsByLayer)
                return new AcadColor(255, 255, 255) { IsByLayer = true };

            if (selectedColor.IsByBlock)
                return new AcadColor(255, 255, 255) { IsByBlock = true };

            AcadColor acadColor = new(selectedColor.ColorValue.R, selectedColor.ColorValue.G, selectedColor.ColorValue.B);

            if (!Colors.Contains(acadColor))
                Colors.Insert(Colors.Count - 1, acadColor);

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

            AcadColor acadColor = new((byte)r, (byte)g, (byte)b);

            if (!Colors.Contains(acadColor))
                Colors.Insert(Colors.Count - 1, acadColor);

            return acadColor;
        }
    }

    /*public string ShowColorDialog(AcadColors acadColors)
    {
        var dialog = new ColorDialog();
        IsDialogOpen = true;

        if (dialog.ShowModal() != true)
        {
            IsDialogOpen = false;
            return string.Empty;
        }

        var selectedColor = dialog.Color;

        if (selectedColor.IsByAci)
        {
            if (selectedColor.IsByLayer)
                return "ByLayer";

            if (selectedColor.IsByBlock)
                return "ByBlock";

            var colorName = $"{selectedColor.ColorValue.R},{selectedColor.ColorValue.G},{selectedColor.ColorValue.B}";

            if (!acadColors.Contains(colorName))
                acadColors.Insert(acadColors.Count - 1, new(colorName, selectedColor.ColorValue.R, selectedColor.ColorValue.G, selectedColor.ColorValue.B));

            IsDialogOpen = false;
            return colorName;
        }
        else
        {
            var colorIndex = selectedColor.ColorIndex;
            var colorByte = Convert.ToByte(colorIndex);
            var rgb = EntityColor.LookUpRgb(colorByte);
            var b = rgb & 0xffL;
            var g = rgb & 0xff00L >> 8;
            var r = rgb >> 16;

            var colorName = $"Color {colorIndex}";

            if (!acadColors.Contains(colorName))
                acadColors.Insert(acadColors.Count - 1, new(colorName, (byte)r, (byte)g, (byte)b));

            IsDialogOpen = false;
            return colorName;
        }
    }*/
}
