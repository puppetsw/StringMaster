using System.Windows.Media;
using Autodesk.Windows.Palettes;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace StringMaster.Common.Helpers;

public static class ColorHelpers
{
    public static SolidColorBrush GetBackgroundColor()
    {
        if (Application.Version.Major <= 19)
            return (SolidColorBrush)new BrushConverter().ConvertFrom(@"#808080");

        if (System.Convert.ToInt32(Application.GetSystemVariable(@"COLORTHEME")) == 1)
        {
            var darkColor = new PaletteTheme(PaletteThemeDefaults.DarkOverallColor);
            return new SolidColorBrush(darkColor.PaletteTabBackgroundColor);
        }
        else
        {
            var lightColor = new PaletteTheme(PaletteThemeDefaults.DarkOverallColor);
            return new SolidColorBrush(lightColor.PaletteTabBackgroundColor);
        }
    }
}
