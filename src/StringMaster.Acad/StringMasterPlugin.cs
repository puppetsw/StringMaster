using Autodesk.AutoCAD.Runtime;
using StringMaster.Acad;
using StringMaster.Common.Services.Implementation;

[assembly: ExtensionApplication(typeof(StringMasterPlugin))]
namespace StringMaster.Acad;

public sealed class StringMasterPlugin : IExtensionApplication
{
    public void Initialize()
    {
        AcadApplicationService.Editor.WriteMessage("\n==========================");
        AcadApplicationService.Editor.WriteMessage("\nStringMaster Loaded (ACAD)");
        AcadApplicationService.Editor.WriteMessage("\n==========================");
    }

    public void Terminate()
    {
    }
}
