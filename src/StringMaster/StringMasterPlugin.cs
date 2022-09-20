using System.Reflection;
using Autodesk.AutoCAD.Runtime;
using StringMaster;
using StringMaster.Dialogs;
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
        Ioc.Default.Register<IMessageBoxService, MessageBoxService>();
        Ioc.Default.Register<IOpenDialogService, OpenDialogService>();
        Ioc.Default.Register<ISaveDialogService, SaveDialogService>();
        Ioc.Default.RegisterSingleton<IAcadColorDialogService, AcadColorDialogService>();
        Ioc.Default.RegisterSingleton<IAcadLayerService, AcadLayerService>();
        Ioc.Default.RegisterSingleton<IDialogService, DialogService>();

        // TODO: ViewModels
        Ioc.Default.Register<StringCogoPointsViewModel>();
        // Ioc.Default.Register<LayerSelectDialogViewModel>();

        // TODO: Views/Controls
        Ioc.Default.Register<StringCogoPointsView>();
        // Ioc.Default.Register<LayerSelectDialog>();

        Ioc.Default.Verify();


    }

    public void Terminate()
    {
    }
}
