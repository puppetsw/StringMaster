using Autodesk.AutoCAD.Runtime;
using StringMaster.Civil;
using StringMaster.Common.Services.Implementation;

[assembly: ExtensionApplication(typeof(StringMasterPlugin))]
namespace StringMaster.Civil;

public sealed class StringMasterPlugin : IExtensionApplication
{
    public void Initialize()
    {
        AcadApplicationService.Editor.WriteMessage("\n===========================");
        AcadApplicationService.Editor.WriteMessage("\nStringMaster Loaded (CIVIL)");
        AcadApplicationService.Editor.WriteMessage("\n===========================");
    }

    public void Terminate()
    {
    }
}
