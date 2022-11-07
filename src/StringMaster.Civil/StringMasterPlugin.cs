using Autodesk.AutoCAD.Runtime;
using StringMaster.Civil;

[assembly: ExtensionApplication(typeof(StringMasterPlugin))]
namespace StringMaster.Civil;

public sealed class StringMasterPlugin : IExtensionApplication
{
    public void Initialize()
    {
    }

    public void Terminate()
    {
    }
}
