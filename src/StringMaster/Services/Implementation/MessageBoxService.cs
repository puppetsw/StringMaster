using System;
using System.Windows;
using StringMaster.Services.Interfaces;

namespace StringMaster.Services.Implementation;

public class MessageBoxService : IMessageBoxService
{
    public bool? ShowYesNo(string title, string message)
    {
        var messageBox = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        switch (messageBox)
        {
            case MessageBoxResult.Yes:
                return true;
            case MessageBoxResult.No:
                return false;
            case MessageBoxResult.None:
            case MessageBoxResult.OK:
            case MessageBoxResult.Cancel:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool? ShowYesNoCancel(string title, string message)
    {
        var messageBox = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        switch (messageBox)
        {
            case MessageBoxResult.Cancel:
                return null;
            case MessageBoxResult.Yes:
                return true;
            case MessageBoxResult.No:
                return false;
            case MessageBoxResult.None:
            case MessageBoxResult.OK:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
