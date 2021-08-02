using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DTO;
using Entities;

namespace BusinessServices.Services
{
    public class LifePointService : ILifePointService
    {
        private readonly IStorage _storage;
        private readonly IMapper _mapper;

        public LifePointService(IStorage storage, IMapper mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }

        public IEnumerable<ExistingLocation> GetAllLocations() => _mapper.Map<IQueryable<LifePoint>, IEnumerable<ExistingLocation>>(_storage.LifePoints);
    }
}