using DTO.Person;

namespace BusinessServices.Services;

public interface IPersonService
{
    Task<ExistingPerson> CreatePersonAsync(PersonToCreate personToCreate);

    bool PersonExists(Guid id);
}
