using System;

// EF Core needs default constructor
#pragma warning disable 8618

namespace Entities
{
    public class Person
    {
        public Person()
        {
        }

        public Person(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}