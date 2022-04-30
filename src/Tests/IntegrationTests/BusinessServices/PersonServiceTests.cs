using System.Threading.Tasks;
using BusinessServices.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.IntegrationTests.BusinessServices;

[TestFixture]
public class PersonServiceTests
{
    [Test]
    public async Task CreateNewPerson()
    {
        var storage = TestStorage.Create();
        var personToCreate = TestPersonToCreate.Create("Bob");
        var testee = new PersonService(new Mock<ILogger<PersonService>>().Object, storage, TestMapper.Create());

        var result = await testee.CreatePersonAsync(personToCreate);

        storage.Persons.Should().ContainSingle(x => x.Id == result.Id);
    }
}