using Autodesk.AutoCAD.Runtime;
using StringMaster.Acad;
using StringMaster.Services.Implementation;
using StringMaster.UI.Services.Interfaces;

[assembly: CommandClass(typeof(Commands))]
namespace StringMaster.Acad;

public static class Commands
{
    private static StringMasterPalette s_palette;

    [CommandMethod("WMS", "_StringMaster", CommandFlags.Modal)]
    public static void ShowStringMaster()
    {
        if (s_palette == null)
            s_palette = new StringMasterPalette(new StringCivilPointService(new ImportService(), new OpenDialogService()));

        s_palette.Visible = true;
    }
}
