using AutoMapper;
using DTO.Person;
using Entities;
using Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace BusinessServices.Services;

public class PersonService(ILogger<PersonService> logger, IStorage storage, IMapper mapper) : IPersonService
{
    public async Task<ExistingPerson> CreatePersonAsync(PersonToCreate personToCreate)
    {
        logger.MethodStarted();

        var newPerson = mapper.Map<Person>(personToCreate);
        var createdPerson = await storage.AddItemAsync(newPerson);
        await storage.SaveAsync();
        logger.NewPersonCreated(createdPerson.Id);

        logger.MethodFinished();
        return mapper.Map<ExistingPerson>(createdPerson);
    }

    /// <inheritdoc />
    public bool PersonExists(Guid id) => storage.Find<Person>(id) != null;
}