using DTO.Person;

namespace Tests.Doubles
{
    public static class TestPersonToCreate
    {
        public static PersonToCreate Create(string name) => new(name);
    }
}