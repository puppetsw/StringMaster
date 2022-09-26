using StringMaster.Services.Implementation;
using StringMaster.Services.Interfaces;

namespace StringMaster;

public static class StaticServices
{
    public static IDialogService DialogService { get; } = new DialogService();

    public static IAcadColorDialogService ColorDialogService { get; } = new AcadColorDialogService();

    public static IAcadLayerService LayerService { get; } = new AcadLayerService();

    public static IAcadLineweightDialogService LineweightDialogService { get; } = new AcadLineweightDialogService();

    public static IAcadLinetypeDialogService LinetypeDialogService { get; } = new AcadLinetypeDialogService();

}