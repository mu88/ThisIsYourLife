using DTO.LifePoint;
using DTO.Location;
using DTO.Person;

namespace BusinessServices.Services;

public interface ILifePointService
{
    IEnumerable<ExistingLocation> GetAllLocations(int? year = null, Guid? creatorId = null);

    Task<ExistingLifePoint> GetLifePointAsync(Guid id);

    Task<ExistingLifePoint> CreateLifePointAsync(LifePointToCreate lifePointToCreate);

    Task DeleteLifePointAsync(Guid id);

    IEnumerable<int> GetDistinctYears(Guid? creatorId = null);

    IEnumerable<ExistingPerson> GetDistinctCreators(int? year = null);
}