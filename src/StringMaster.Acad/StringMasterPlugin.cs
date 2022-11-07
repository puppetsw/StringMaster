using Autodesk.AutoCAD.Runtime;
using StringMaster.Acad;

[assembly: ExtensionApplication(typeof(StringMasterPlugin))]
namespace StringMaster.Acad;

public sealed class StringMasterPlugin : IExtensionApplication
{
    public void Initialize()
    {
    }

    public void Terminate()
    {
    }
}
