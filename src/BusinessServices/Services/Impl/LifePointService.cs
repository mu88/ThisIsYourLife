﻿using System.Linq.Expressions;
using AutoMapper;
using DTO.LifePoint;
using DTO.Location;
using DTO.Person;
using Entities;
using Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace BusinessServices.Services;

internal class LifePointService : ILifePointService
{
    private readonly ILogger<LifePointService> _logger;
    private readonly IStorage _storage;
    private readonly IMapper _mapper;

    public LifePointService(ILogger<LifePointService> logger, IStorage storage, IMapper mapper)
    {
        _logger = logger;
        _storage = storage;
        _mapper = mapper;
    }

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

        return _mapper.Map<IQueryable<LifePoint>, IEnumerable<ExistingLocation>>(searchExpression != null
                                                                                     ? _storage.LifePoints.Where(searchExpression)
                                                                                     : _storage.LifePoints);
    }

    public async Task<ExistingLifePoint> GetLifePointAsync(Guid id) => _mapper.Map<LifePoint, ExistingLifePoint>(await GetLifePointInternalAsync(id));

    public async Task<ExistingLifePoint> CreateLifePointAsync(LifePointToCreate lifePointToCreate)
    {
        _logger.MethodStarted();

        var existingPerson = await _storage.FindAsync<Person>(lifePointToCreate.CreatedBy) ??
                             throw new ArgumentNullException(nameof(lifePointToCreate), $"Could not find any existing Person with ID {lifePointToCreate.CreatedBy}");

        Guid? imageId = lifePointToCreate.ImageToCreate != null ? await _storage.StoreImageAsync(existingPerson, lifePointToCreate.ImageToCreate) : null;
        var newLifePoint = _mapper.Map<LifePointToCreate, LifePoint>(lifePointToCreate,
                                                                     options =>
                                                                     {
                                                                         options.Items[nameof(LifePoint.CreatedBy)] = existingPerson;
                                                                         options.Items[nameof(LifePoint.ImageId)] = imageId;
                                                                     });
        _logger.NewLifePointExtracted();

        var createdLifePoint = await _storage.AddItemAsync(newLifePoint);
        await _storage.SaveAsync();
        _logger.NewLifePointCreated(createdLifePoint.Id);

        _logger.MethodFinished();

        return _mapper.Map<LifePoint, ExistingLifePoint>(createdLifePoint);
    }

    public async Task DeleteLifePointAsync(Guid id)
    {
        _logger.MethodStarted();

        var lifePoint = await GetLifePointInternalAsync(id);
        _storage.RemoveItem(lifePoint);
        if (lifePoint.ImageId != null) _storage.DeleteImage(lifePoint.CreatedBy.Id, lifePoint.ImageId.Value);
        await _storage.SaveAsync();
        _logger.LifePointDeleted(id);

        _logger.MethodFinished();
    }

    public IEnumerable<int> GetDistinctYears(Guid? creatorId = null) => creatorId != null
                                                                     ? _storage.LifePoints.Where(point => point.CreatedBy.Id == creatorId)
                                                                         .Select(x => x.Date.Year)
                                                                         .Distinct()
                                                                         .OrderBy(x => x)
                                                                     : _storage.LifePoints.Select(x => x.Date.Year).Distinct().OrderBy(x => x);

    public IEnumerable<ExistingPerson> GetDistinctCreators(int? year = null)
    {
        var distinctCreators = year != null
                                   ? _storage.LifePoints.Where(point => point.Date.Year == year).Select(x => x.CreatedBy).Distinct().OrderBy(x => x.Name)
                                   : _storage.LifePoints.Select(x => x.CreatedBy).Distinct().OrderBy(x => x.Name);
        return _mapper.Map<IQueryable<Person>, IEnumerable<ExistingPerson>>(distinctCreators);
    }

    private async Task<LifePoint> GetLifePointInternalAsync(Guid id) =>
        await _storage.FindAsync<LifePoint>(id) ??
        throw new ArgumentNullException(nameof(id), $"Could not find any existing LifePoint with ID {id}");
}