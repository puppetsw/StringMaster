#nullable enable

using System.Collections.ObjectModel;
using StringMaster.Models;

namespace StringMaster.Services.Interfaces;

public interface IAcadColorDialogService
{
    ObservableCollection<AcadColor> Colors { get; }

    AcadColor? ShowDialog();
}
