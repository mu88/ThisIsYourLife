using System.Collections.Generic;
using DTO;

namespace BusinessServices.Services
{
    public interface ILifePointService
    {
        IEnumerable<ExistingLocation> GetAllLocations();
    }
}