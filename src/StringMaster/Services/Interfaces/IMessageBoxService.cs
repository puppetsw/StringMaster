namespace StringMaster.Services.Interfaces;

public interface IMessageBoxService
{
    public bool? ShowYesNo(string title, string message);
    public bool? ShowYesNoCancel(string title, string message);
    public void ShowError(string title, string message);
    public void ShowWarning(string title, string message);
}