using AutoMapper;
using BusinessServices;
using BusinessServices.Services;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.UnitTests.BusinessServices;

[TestFixture]
[Category("Unit")]
public class PersonServiceTests
{
    private readonly IStorage _storage = Substitute.For<IStorage>();
    private readonly IMapper _mapper = TestMapper.Create();

    [Test]
    public async Task CreateNewPerson()
    {
        var personToCreate = TestPersonToCreate.Create("Bob");
        _storage.AddItemAsync(Arg.Any<Person>()).Returns(_mapper.Map<Person>(personToCreate));
        var testee = CreateTestee();

        var result = await testee.CreatePersonAsync(personToCreate);

        result.Id.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(personToCreate);
        await _storage.Received(1).SaveAsync();
    }

    [Test]
    public void PersonDoesNotExist()
    {
        var id = Guid.NewGuid();
        _storage.Find<Person>(id).Returns((Person?)null);
        var testee = CreateTestee();

        var result = testee.PersonExists(id);

        result.Should().BeFalse();
    }

    [Test]
    public void PersonExists()
    {
        var person = TestPerson.Create("Dixie");
        _storage.Find<Person>(person.Id).Returns(person);
        var testee = CreateTestee();

        var result = testee.PersonExists(person.Id);

        result.Should().BeTrue();
    }

    private PersonService CreateTestee() => new(Substitute.For<ILogger<PersonService>>(), _storage, _mapper);
}