namespace StringMaster.Services.Interfaces;

public interface ISaveDialogService
{
    bool? ShowDialog();

    string FileName { get; set; }

    string DefaultExt { get; set; }

    string Filter { get; set; }
}