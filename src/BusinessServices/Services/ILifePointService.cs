using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTO.LifePoint;
using DTO.Location;

namespace BusinessServices.Services
{
    public interface ILifePointService
    {
        IEnumerable<ExistingLocation> GetAllLocations();

        Task<ExistingLifePoint> GetLifePointAsync(Guid id);

        Task<ExistingLifePoint> CreateLifePointAsync(LifePointToCreate lifePointToCreate);
    }
}