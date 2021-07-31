using System;

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