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
        // TODO: Services
        Ioc.Register<IMessageBoxService, MessageBoxService>();
        Ioc.Register<IOpenDialogService, OpenDialogService>();
        Ioc.Register<ISaveDialogService, SaveDialogService>();
        Ioc.Register<IAcadColorPicker, AcadColorPicker>();
        Ioc.Register<IAcadLayerService, AcadLayerService>();

        // TODO: ViewModels
        Ioc.Register<StringCogoPointsViewModel>();

        // TODO: Views/Controls
        Ioc.Register<StringCogoPointsView>();
    }

    public void Terminate()
    {
    }
}
