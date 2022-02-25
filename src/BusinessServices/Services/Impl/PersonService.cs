using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DTO.Person;
using Entities;

namespace BusinessServices.Services;

public class PersonService
    : IPersonService
{
    private readonly IStorage _storage;
    private readonly IMapper _mapper;

    public PersonService(IStorage storage, IMapper mapper)
    {
        _storage = storage;
        _mapper = mapper;
    }

    public async Task<ExistingPerson> CreatePersonAsync(PersonToCreate personToCreate)
    {
        var newPerson = _mapper.Map<Person>(personToCreate);
        var createdPerson = await _storage.AddItemAsync(newPerson);
        await _storage.SaveAsync();

        return _mapper.Map<ExistingPerson>(createdPerson);
    }

    /// <inheritdoc />
    public IEnumerable<ExistingPerson> GetAllPersons() => _mapper.Map<IQueryable<Person>, IEnumerable<ExistingPerson>>(_storage.Persons);

    /// <inheritdoc />
    public bool PersonExists(Guid id) => _storage.Find<Person>(id) != null;
}