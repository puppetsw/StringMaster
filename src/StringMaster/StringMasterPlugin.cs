using Autodesk.AutoCAD.Runtime;
using StringMaster;
using StringMaster.Services.Implementation;
using StringMaster.Services.Interfaces;
using StringMaster.UserControls;
using StringMaster.ViewModels;

[assembly: ExtensionApplication(typeof(StringMasterPlugin))]
namespace StringMaster;

public sealed class StringMasterPlugin : IExtensionApplication
{
    public void Initialize()
    {
        Ioc.Register<IMessageBoxService, MessageBoxService>();
        Ioc.Register<IOpenDialogService, OpenDialogService>();
        Ioc.Register<ISaveDialogService, SaveDialogService>();
        Ioc.Register<StringCogoPointsViewModel>();
        Ioc.Register<StringCogoPoints>();
    }

    public void Terminate()
    {
    }
}
