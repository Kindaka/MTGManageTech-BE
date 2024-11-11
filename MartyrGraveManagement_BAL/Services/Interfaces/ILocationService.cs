using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ILocationService
    {
        Task<(bool status, string message)> ImportLocationsFromExcelAsync(string excelFilePath);
    }
}
