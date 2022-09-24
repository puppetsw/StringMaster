#nullable enable

using System;
using System.Collections.ObjectModel;
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
public class AcadColorDialogService : IAcadColorDialogService
{
    public ObservableCollection<AcadColor> Colors { get; } = new();

    public AcadColorDialogService()
    {
        // Add default ACAD colors
        Colors.Add(AcadColor.ByLayer);              // ByLayer
        Colors.Add(AcadColor.ByBlock);              // ByBlock
        Colors.Add(new AcadColor(255, 0, 0));       // Red
        Colors.Add(new AcadColor(255, 255, 0));     // Yellow
        Colors.Add(new AcadColor(0, 255, 0));       // Green
        Colors.Add(new AcadColor(0, 255, 255));     // Cyan
        Colors.Add(new AcadColor(0, 0, 255));       // Blue
        Colors.Add(new AcadColor(255, 0, 255));     // Magenta
        Colors.Add(new AcadColor(255, 255, 255));   // White
        // Add an extra ComboBox Item so we can 'choose' the color with the dialog.
        Colors.Add(AcadColor.ColorPicker);
    }

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

            AcadColor acadColor = new((byte)r, (byte)g, (byte)b, colorIndex);

            if (!Colors.Contains(acadColor))
                Colors.Insert(Colors.Count - 1, acadColor);

            return acadColor;
        }
    }
}
