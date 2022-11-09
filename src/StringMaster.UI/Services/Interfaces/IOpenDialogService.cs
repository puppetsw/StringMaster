namespace StringMaster.UI.Services.Interfaces;

public interface IOpenDialogService
{
    bool? ShowDialog();

    string FileName { get; set; }

    string DefaultExt { get; set; }

    string Filter { get; set; }
}