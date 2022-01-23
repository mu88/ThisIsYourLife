using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTO.LifePoint;
using DTO.Location;
using DTO.Person;

namespace BusinessServices.Services;

public interface ILifePointService
{
    IEnumerable<ExistingLocation> GetAllLocations();

    Task<ExistingLifePoint> GetLifePointAsync(Guid id);

    Task<ExistingLifePoint> CreateLifePointAsync(LifePointToCreate lifePointToCreate);

    Task DeleteLifePointAsync(Guid id);

    IEnumerable<int> GetDistinctYears();

    IEnumerable<ExistingPerson> GetDistinctCreators();

    IEnumerable<ExistingLocation> GetAllLocations(uint year);

    IEnumerable<ExistingLocation> GetAllLocations(Guid creatorId);
}