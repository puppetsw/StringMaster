#nullable enable

using StringMaster.UI.Models;

namespace StringMaster.UI.Services.Interfaces;

public interface IAcadColorDialogService
{
    AcadColor? ShowDialog();
}
