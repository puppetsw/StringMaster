namespace StringMaster.UI.Services.Interfaces;

/// <summary>
/// Services to be implemented by each version of the plugin.
/// </summary>
public interface IServiceCollection
{
    IAcadApplicationService AcadApplicationService { get; }

    IAcadLayerService AcadLayerService { get; }

    IAcadColorDialogService AcadColorDialogService { get; }

    IAcadLinetypeDialogService AcadLinetypeDialogService { get; }

    IAcadLineweightDialogService AcadLineweightDialogService { get; }

    IDialogService DialogService { get; }

    IMessageBoxService MessageBoxService { get; }

    IOpenDialogService OpenDialogService { get; }

    ISaveDialogService SaveDialogService { get; }

    IStringCivilPointsService StringCivilPointsService { get; }
}
