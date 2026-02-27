using Entities;

namespace Tests.Doubles;

public static class TestPerson
{
    public static Person Create(string name) => new(name);
}
