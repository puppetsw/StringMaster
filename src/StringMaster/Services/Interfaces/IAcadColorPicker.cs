using StringMaster.Models;

namespace StringMaster.Services.Interfaces;

public interface IAcadColorPicker
{
    AcadColors Colors { get; }

    AcadColor GetAcadColor();
}
