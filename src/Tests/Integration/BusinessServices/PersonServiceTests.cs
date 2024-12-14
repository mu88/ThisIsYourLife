using BusinessServices.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.Integration.BusinessServices;

[TestFixture]
[Category("Integration")]
public class PersonServiceTests
{
    [Test]
    public async Task CreateNewPerson()
    {
        var storage = TestStorage.Create();
        var personToCreate = TestPersonToCreate.Create("Bob");
        var testee = new PersonService(Substitute.For<ILogger<PersonService>>(), storage, TestMapper.Create());

        var result = await testee.CreatePersonAsync(personToCreate);

        storage.Persons.Should().ContainSingle(x => x.Id == result.Id);
    }
}