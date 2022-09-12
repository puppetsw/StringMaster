using Autodesk.AutoCAD.Runtime;
using StringMaster;

[assembly: ExtensionApplication(typeof(Plugin))]
namespace StringMaster;

public sealed class Plugin : IExtensionApplication
{
    public void Initialize()
    {
    }

    public void Terminate()
    {
    }
}
