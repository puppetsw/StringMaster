using System.Collections.Generic;
using StringMaster.UI.Models;

namespace StringMaster.UI.Services.Interfaces;

public interface IStringCivilPointsService
{
    void StringCivilPoints(IList<DescriptionKey> descriptionKeys);
}
