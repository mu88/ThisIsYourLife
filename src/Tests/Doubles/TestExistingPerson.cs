using System;
using DTO.Person;

namespace Tests.Doubles;

public static class TestExistingPerson
{
    public static ExistingPerson Create(string name) => new(Guid.NewGuid(), name);
}