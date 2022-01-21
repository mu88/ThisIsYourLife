using System.Collections.Generic;
using System.Threading.Tasks;
using DTO.Person;

namespace BusinessServices.Services;

public interface IPersonService
{
    Task<ExistingPerson> CreatePersonAsync(PersonToCreate personToCreate);

    // TODO mu88: Add tests if necessary in production
    IEnumerable<ExistingPerson> GetAllPersons();
}