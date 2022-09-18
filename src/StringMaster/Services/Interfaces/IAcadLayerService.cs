using System.Collections.ObjectModel;
using StringMaster.Models;

namespace StringMaster.Services.Interfaces;

public interface IAcadLayerService
{
    ObservableCollection<AcadLayer> Layers { get; }
}