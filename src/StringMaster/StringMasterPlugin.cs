using Autodesk.AutoCAD.Runtime;
using StringMaster;

[assembly: ExtensionApplication(typeof(StringMasterPlugin))]
namespace StringMaster;

public sealed class StringMasterPlugin : IExtensionApplication
{
    public void Initialize()
    {
    }

    public void Terminate()
    {
    }
}
