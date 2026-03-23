using DTO.LifePoint;
using DTO.Location;
using DTO.Person;
using Entities;

namespace BusinessServices;

public static class MappingExtensions
{
    public static Person ToPerson(this PersonToCreate personToCreate) =>
        new(personToCreate.Name);

    public static ExistingPerson ToExistingPerson(this Person person) =>
        new(person.Id, person.Name);

    public static ExistingLocation ToExistingLocation(this LifePoint lifePoint) =>
        new(lifePoint.Latitude, lifePoint.Longitude, lifePoint.Id);

    public static ExistingLifePoint ToExistingLifePoint(this LifePoint lifePoint) =>
        new(
            lifePoint.Id,
            lifePoint.Date,
            lifePoint.Caption,
            lifePoint.Description,
            lifePoint.Latitude,
            lifePoint.Longitude,
            lifePoint.CreatedBy.ToExistingPerson(),
            lifePoint.ImageId);

    public static LifePoint ToLifePoint(this LifePointToCreate lifePointToCreate, Person createdBy, Guid? imageId = null) =>
        new(
            lifePointToCreate.Date,
            lifePointToCreate.Caption,
            lifePointToCreate.Description,
            lifePointToCreate.Latitude,
            lifePointToCreate.Longitude,
            createdBy,
            imageId);

    public static IEnumerable<ExistingLocation> ToExistingLocations(this IQueryable<LifePoint>? lifePoints) =>
        lifePoints != null
            ? lifePoints.Select(lifePoint => lifePoint.ToExistingLocation())
            : Enumerable.Empty<ExistingLocation>();

    public static IEnumerable<ExistingPerson> ToExistingPersons(this IQueryable<Person>? persons) =>
        persons != null
            ? persons.Select(person => person.ToExistingPerson())
            : Enumerable.Empty<ExistingPerson>();
}
