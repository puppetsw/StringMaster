using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(StringMaster.Commands))]
namespace StringMaster;

public static class Commands
{
    private static MyPaletteSet s_palette;

    [CommandMethod("WMS", "_StringMaster", CommandFlags.Modal)]
    public static void ShowStringMaster()
    {
        if (s_palette == null)
            s_palette = new MyPaletteSet();

        s_palette.Visible = true;
    }
}
