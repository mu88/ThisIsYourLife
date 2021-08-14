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
            _mapper.Map<LifePoint, ExistingLifePoint>(await _storage.FindAsync<LifePoint>(id) ??
                                                      throw new NullReferenceException($"Could not find any existing LifePoint with ID {id}"));

        public async Task<ExistingLifePoint> CreateLifePointAsync(LifePointToCreate lifePointToCreate)
        {
            var existingPerson = await _storage.FindAsync<Person>(lifePointToCreate.CreatedBy);
            var newLifePoint = _mapper.Map<LifePointToCreate, LifePoint>(lifePointToCreate, options => options.Items[nameof(LifePoint.CreatedBy)] = existingPerson);
            var createdLifePoint = await _storage.AddItemAsync(newLifePoint);
            await _storage.SaveAsync();

            return _mapper.Map<LifePoint, ExistingLifePoint>(createdLifePoint);
        }
    }
}