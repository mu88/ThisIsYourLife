using System.Linq.Expressions;
using AutoMapper;
using DTO.LifePoint;
using DTO.Location;
using DTO.Person;
using Entities;
using Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace BusinessServices.Services;

internal class LifePointService(ILogger<LifePointService> logger, IStorage storage, IMapper mapper) : ILifePointService
{
    public IEnumerable<ExistingLocation> GetAllLocations(int? year = null, Guid? creatorId = null)
    {
        Expression<Func<LifePoint, bool>>? searchExpression;

        if (creatorId != null)
        {
            if (year != null)
                searchExpression = point => point.Date.Year == year && point.CreatedBy.Id == creatorId;
            else
                searchExpression = point => point.CreatedBy.Id == creatorId;
        }
        else
        {
            if (year != null)
                searchExpression = point => point.Date.Year == year;
            else
                searchExpression = null;
        }

        return mapper.Map<IQueryable<LifePoint>, IEnumerable<ExistingLocation>>(searchExpression != null
                                                                                     ? storage.LifePoints.Where(searchExpression)
                                                                                     : storage.LifePoints);
    }

    public async Task<ExistingLifePoint> GetLifePointAsync(Guid id) => mapper.Map<LifePoint, ExistingLifePoint>(await GetLifePointInternalAsync(id));

    public async Task<ExistingLifePoint> CreateLifePointAsync(LifePointToCreate lifePointToCreate)
    {
        logger.MethodStarted();

        var existingPerson = await storage.FindAsync<Person>(lifePointToCreate.CreatedBy) ??
                             throw new ArgumentNullException(nameof(lifePointToCreate), $"Could not find any existing Person with ID {lifePointToCreate.CreatedBy}");

        Guid? imageId = lifePointToCreate.ImageToCreate != null ? await storage.StoreImageAsync(existingPerson, lifePointToCreate.ImageToCreate) : null;
        var newLifePoint = mapper.Map<LifePointToCreate, LifePoint>(lifePointToCreate,
                                                                     options =>
                                                                     {
                                                                         options.Items[nameof(LifePoint.CreatedBy)] = existingPerson;
                                                                         options.Items[nameof(LifePoint.ImageId)] = imageId;
                                                                     });
        logger.NewLifePointExtracted();

        var createdLifePoint = await storage.AddItemAsync(newLifePoint);
        await storage.SaveAsync();
        logger.NewLifePointCreated(createdLifePoint.Id);

        logger.MethodFinished();

        var existingLifePoint = mapper.Map<LifePoint, ExistingLifePoint>(createdLifePoint);
        return existingLifePoint;
    }

    public async Task DeleteLifePointAsync(Guid id)
    {
        logger.MethodStarted();

        var lifePoint = await GetLifePointInternalAsync(id);
        storage.RemoveItem(lifePoint);
        if (lifePoint.ImageId != null) storage.DeleteImage(lifePoint.CreatedBy.Id, lifePoint.ImageId.Value);
        await storage.SaveAsync();
        logger.LifePointDeleted(id);

        logger.MethodFinished();
    }

    public IEnumerable<int> GetDistinctYears(Guid? creatorId = null) => creatorId != null
                                                                     ? storage.LifePoints.Where(point => point.CreatedBy.Id == creatorId)
                                                                         .Select(x => x.Date.Year)
                                                                         .Distinct()
                                                                         .OrderBy(x => x)
                                                                     : storage.LifePoints.Select(x => x.Date.Year).Distinct().OrderBy(x => x);

    public IEnumerable<ExistingPerson> GetDistinctCreators(int? year = null)
    {
        var distinctCreators = year != null
                                   ? storage.LifePoints.Where(point => point.Date.Year == year).Select(x => x.CreatedBy).Distinct().OrderBy(x => x.Name)
                                   : storage.LifePoints.Select(x => x.CreatedBy).Distinct().OrderBy(x => x.Name);
        return mapper.Map<IQueryable<Person>, IEnumerable<ExistingPerson>>(distinctCreators);
    }

    private async Task<LifePoint> GetLifePointInternalAsync(Guid id) =>
        await storage.FindAsync<LifePoint>(id) ??
        throw new ArgumentNullException(nameof(id), $"Could not find any existing LifePoint with ID {id}");
}