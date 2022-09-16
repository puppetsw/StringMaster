using System.Collections.ObjectModel;
using System.Linq;

namespace StringMaster.Models;

public sealed class AcadColors : ObservableCollection<AcadColor>
{
    public AcadColors()
    {
        Add(new AcadColor(255, 255, 255) { IsByLayer = true });
        Add(new AcadColor(255, 255, 255) { IsByBlock = true });
        Add(new AcadColor(255, 0, 0));
        Add(new AcadColor(255, 255, 0));
        Add(new AcadColor(0, 255, 0));
        Add(new AcadColor(0, 255, 255));
        Add(new AcadColor(0, 0, 255));
        Add(new AcadColor(255, 0, 255));
        Add(new AcadColor(255, 255, 255));
        Add(new AcadColor(255, 255, 255) { IsColorPicker = true });
    }
}
