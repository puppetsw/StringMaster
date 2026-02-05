using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMaster.UI.Services.Interfaces
{
	public interface ICivilPointGroupService
	{
		IEnumerable<string> GetPointGroups();
	}
}
