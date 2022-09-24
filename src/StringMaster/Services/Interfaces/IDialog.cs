using System.ComponentModel;

namespace StringMaster.Services.Interfaces;

public interface IDialog<TViewModel> where TViewModel : class, INotifyPropertyChanged
{
    TViewModel ViewModel { get; set; }
}