using DTO.Person;
using Entities;
using Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace BusinessServices.Services;

internal class PersonService(ILogger<PersonService> logger, IStorage storage) : IPersonService
{
    public async Task<ExistingPerson> CreatePersonAsync(PersonToCreate personToCreate)
    {
        using var activity = Tracing.Source.StartActivity();

        var newPerson = personToCreate.ToPerson();
        var createdPerson = await storage.AddItemAsync(newPerson);
        await storage.SaveAsync();
        logger.NewPersonCreated(createdPerson.Id);

        return createdPerson.ToExistingPerson();
    }

    /// <inheritdoc />
    public bool PersonExists(Guid id) => storage.Find<Person>(id) != null;
}
