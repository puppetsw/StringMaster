using System.ComponentModel;

namespace StringMaster.UI.Services.Interfaces;

public interface IDialog<TViewModel> where TViewModel : class, INotifyPropertyChanged
{
    TViewModel ViewModel { get; set; }
}