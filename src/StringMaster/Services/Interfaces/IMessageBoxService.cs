namespace StringMaster.Services.Interfaces;

public interface IMessageBoxService
{
    public bool? ShowYesNo(string title, string message);

    public bool? ShowYesNoCancel(string title, string message);
}