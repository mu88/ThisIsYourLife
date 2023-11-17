using AutoMapper;
using DTO.Person;
using Entities;
using Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace BusinessServices.Services;

public class PersonService
    : IPersonService
{
    private readonly ILogger<PersonService> _logger;
    private readonly IStorage _storage;
    private readonly IMapper _mapper;

    public PersonService(ILogger<PersonService> logger, IStorage storage, IMapper mapper)
    {
        _logger = logger;
        _storage = storage;
        _mapper = mapper;
    }

    public async Task<ExistingPerson> CreatePersonAsync(PersonToCreate personToCreate)
    {
        _logger.MethodStarted();

        var newPerson = _mapper.Map<Person>(personToCreate);
        var createdPerson = await _storage.AddItemAsync(newPerson);
        await _storage.SaveAsync();
        _logger.NewPersonCreated(createdPerson.Id);

        _logger.MethodFinished();
        return _mapper.Map<ExistingPerson>(createdPerson);
    }

    /// <inheritdoc />
    public bool PersonExists(Guid id) => _storage.Find<Person>(id) != null;
}