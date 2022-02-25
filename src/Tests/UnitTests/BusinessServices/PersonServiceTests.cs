using System;
using System.Threading.Tasks;
using BusinessServices;
using BusinessServices.Services;
using Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.UnitTests.BusinessServices;

[TestFixture]
public class PersonServiceTests
{
    [Test]
    public async Task CreateNewPerson()
    {
        var personToCreate = TestPersonToCreate.Create("Bob");
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<Person>>(x => x.AddItemAsync(It.IsAny<Person>())).Returns<Person>(Task.FromResult);
        var testee = autoMocker.CreateInstance<PersonService>();

        var result = await testee.CreatePersonAsync(personToCreate);

        result.Id.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(personToCreate);
        autoMocker.Verify<IStorage>(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public void PersonDoesNotExist()
    {
        var id = Guid.NewGuid();
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Person?>(storage => storage.Find<Person>(id)).Returns((Person?)null);
        var testee = autoMocker.CreateInstance<PersonService>();

        var result = testee.PersonExists(id);

        result.Should().BeFalse();
    }

    [Test]
    public void PersonExists()
    {
        var person = TestPerson.Create("Dixie");
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Person?>(storage => storage.Find<Person>(person.Id)).Returns(person);
        var testee = autoMocker.CreateInstance<PersonService>();

        var result = testee.PersonExists(person.Id);

        result.Should().BeTrue();
    }
}