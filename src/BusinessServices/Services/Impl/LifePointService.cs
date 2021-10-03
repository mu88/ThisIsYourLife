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

        public async Task<ExistingLifePoint> GetLifePointAsync(Guid id) => _mapper.Map<LifePoint, ExistingLifePoint>(await GetLifePointInternalAsync(id));

        public async Task<ExistingLifePoint> CreateLifePointAsync(LifePointToCreate lifePointToCreate)
        {
            var existingPerson = await _storage.FindAsync<Person>(lifePointToCreate.CreatedBy) ??
                                 throw new NullReferenceException($"Could not find any existing Person with ID {lifePointToCreate.CreatedBy}");

            Guid? imageId = lifePointToCreate.ImageStream != null ? await _storage.StoreImageAsync(lifePointToCreate.ImageStream) : null;
            LifePoint newLifePoint = _mapper.Map<LifePointToCreate, LifePoint>(lifePointToCreate,
                                                                               options =>
                                                                               {
                                                                                   options.Items[nameof(LifePoint.CreatedBy)] = existingPerson;
                                                                                   options.Items[nameof(LifePoint.ImageId)] = imageId;
                                                                               });

            var createdLifePoint = await _storage.AddItemAsync(newLifePoint);
            await _storage.SaveAsync();

            return _mapper.Map<LifePoint, ExistingLifePoint>(createdLifePoint);
        }

        public async Task DeleteLifePointAsync(Guid id)
        {
            _storage.RemoveItem(await GetLifePointInternalAsync(id));
            await _storage.SaveAsync();
        }

        private async Task<LifePoint> GetLifePointInternalAsync(Guid id) =>
            await _storage.FindAsync<LifePoint>(id) ??
            throw new NullReferenceException($"Could not find any existing LifePoint with ID {id}");
    }
}