using System.Collections.ObjectModel;
using StringMaster.Models;

namespace StringMaster.Services.Interfaces;

public interface IAcadColorPicker
{
    ObservableCollection<AcadColor> Colors { get; }

    AcadColor GetAcadColor();
}
