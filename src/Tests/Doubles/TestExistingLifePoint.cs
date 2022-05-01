using System;
using DTO.LifePoint;
using DTO.Person;

namespace Tests.Doubles;

public static class TestExistingLifePoint
{
    public static ExistingLifePoint Create()
    {
        var existingPerson = new ExistingPerson(Guid.NewGuid(), "Dixie");
        return new ExistingLifePoint(Guid.NewGuid(),
                                     DateOnly.FromDateTime(DateTime.Now),
                                     "Dynamo",
                                     "Dresden",
                                     Random.Shared.Next(-90, 90),
                                     Random.Shared.Next(-180, 180),
                                     existingPerson,
                                     Guid.NewGuid());
    }

    public static ExistingLifePoint From(LifePointToCreate lifePointToCreate) =>
        new(Guid.Empty,
            lifePointToCreate.Date,
            lifePointToCreate.Caption,
            lifePointToCreate.Description,
            lifePointToCreate.Latitude,
            lifePointToCreate.Longitude,
            new ExistingPerson(lifePointToCreate.CreatedBy, "Dixie"),
            lifePointToCreate.ImageToCreate != null ? Guid.NewGuid() : null);
}