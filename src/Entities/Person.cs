using System.Diagnostics.CodeAnalysis;

// EF Core needs default constructor
#pragma warning disable 8618

namespace Entities;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "EF Core uses them")]
public class Person
{
    // ReSharper disable once UnusedMember.Global - EF Core needs default constructor
    [ExcludeFromCodeCoverage]
    public Person()
    {
    }

    public Person(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }
}
