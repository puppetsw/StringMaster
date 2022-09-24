#nullable enable

using StringMaster.Models;

namespace StringMaster.Services.Interfaces;

public interface IAcadColorDialogService
{
    AcadColor? ShowDialog();
}
