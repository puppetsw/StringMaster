using StringMaster.Services.Implementation;
using StringMaster.UI.Services.Interfaces;

namespace StringMaster;

public static class StaticServices
{
    public static IDialogService DialogService { get; } = new DialogService();

    public static IAcadColorDialogService AcadColorDialogService { get; } = new AcadColorDialogService();

    public static IAcadLayerService AcadLayerService { get; } = new AcadLayerService();

    public static IAcadLineweightDialogService AcadLineweightDialogService { get; } = new AcadLineweightDialogService();

    public static IAcadLinetypeDialogService AcadLinetypeDialogService { get; } = new AcadLinetypeDialogService();

    public static IAcadApplicationService AcadApplicationService { get; } = new AcadApplicationService();
}
