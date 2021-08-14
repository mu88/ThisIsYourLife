using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DTO.LifePoint;
using DTO.Location;
using Entities;

namespace BusinessServices.Services
{
    internal class LifePointService : ILifePointService
    {
        private readonly IStorage _storage;
        private readonly IMapper _mapper;

        public LifePointService(IStorage storage, IMapper mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }

        public IEnumerable<ExistingLocation> GetAllLocations() => _mapper.Map<IQueryable<LifePoint>, IEnumerable<ExistingLocation>>(_storage.LifePoints);

        public async Task<ExistingLifePoint> GetLifePointAsync(Guid id) =>
            CreateExistingLifePoint(await _storage.FindAsync<LifePoint>(id) ??
                                    throw new NullReferenceException($"Could not find any existing LifePoint with ID {id}"));

        public async Task<ExistingLifePoint> CreateLifePointAsync(LifePointToCreate lifePointToCreate)
        {
            var existingPerson = await _storage.FindAsync<Person>(lifePointToCreate.CreatedBy) ??
                                 throw new NullReferenceException($"Could not find any existing Person with ID {lifePointToCreate.CreatedBy}");
            // var newLifePoint = _mapper.Map<LifePointToCreate, LifePoint>(lifePointToCreate, options => options.Items[nameof(LifePoint.CreatedBy)] = existingPerson.Name);
            // TODO mu88: Use AutoMapper as soon as bug is fixed
            var newLifePoint = new LifePoint(lifePointToCreate.Date,
                                             lifePointToCreate.Caption,
                                             lifePointToCreate.Description,
                                             lifePointToCreate.Latitude,
                                             lifePointToCreate.Longitude,
                                             existingPerson);
            var createdLifePoint = await _storage.AddItemAsync(newLifePoint);
            await _storage.SaveAsync();

            return CreateExistingLifePoint(createdLifePoint);
        }

        private static ExistingLifePoint CreateExistingLifePoint(LifePoint lifePoint) =>
            // TODO mu88: Use AutoMapper as soon as bug is fixed
            // return _mapper.Map<LifePoint, ExistingLifePoint>(createdLifePoint);
            new(lifePoint.Id,
                lifePoint.Date,
                lifePoint.Caption,
                lifePoint.Description,
                lifePoint.Latitude,
                lifePoint.Longitude,
                lifePoint.CreatedBy.Name);
    }
}