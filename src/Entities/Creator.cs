using System;

namespace Entities
{
    public class Creator
    {
        public Creator(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}