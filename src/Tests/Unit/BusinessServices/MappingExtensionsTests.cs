using BusinessServices;
using DTO.LifePoint;
using Entities;
using FluentAssertions;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.Unit.BusinessServices;

[TestFixture]
[Category("Unit")]
public class MappingExtensionsTests
{
    [Test]
    public void PersonToCreate_ToPerson()
    {
        var personToCreate = TestPersonToCreate.Create("Bob");

        var result = personToCreate.ToPerson();

        result.Name.Should().Be("Bob");
        result.Id.Should().NotBeEmpty();
    }

    [Test]
    public void Person_ToExistingPerson()
    {
        var person = TestPerson.Create("Dixie");

        var result = person.ToExistingPerson();

        result.Id.Should().Be(person.Id);
        result.Name.Should().Be("Dixie");
    }

    [Test]
    public void LifePoint_ToExistingLocation()
    {
        var lifePoint = TestLifePoint.Create();

        var result = lifePoint.ToExistingLocation();

        result.Id.Should().Be(lifePoint.Id);
        result.Latitude.Should().Be(lifePoint.Latitude);
        result.Longitude.Should().Be(lifePoint.Longitude);
    }

    [Test]
    public void LifePoint_ToExistingLifePoint()
    {
        var person = TestPerson.Create("Ulf");
        var imageId = Guid.NewGuid();
        var lifePoint = TestLifePoint.Create(person, new DateOnly(1953, 4, 12), imageId);

        var result = lifePoint.ToExistingLifePoint();

        result.Id.Should().Be(lifePoint.Id);
        result.Date.Should().Be(new DateOnly(1953, 4, 12));
        result.Caption.Should().Be(lifePoint.Caption);
        result.Description.Should().Be(lifePoint.Description);
        result.Latitude.Should().Be(lifePoint.Latitude);
        result.Longitude.Should().Be(lifePoint.Longitude);
        result.CreatedBy.Id.Should().Be(person.Id);
        result.CreatedBy.Name.Should().Be("Ulf");
        result.ImageId.Should().Be(imageId);
    }

    [Test]
    public void LifePoint_ToExistingLifePoint_WithoutImage()
    {
        var lifePoint = TestLifePoint.Create();

        var result = lifePoint.ToExistingLifePoint();

        result.ImageId.Should().BeNull();
    }

    [Test]
    public void LifePointToCreate_ToLifePoint()
    {
        var person = TestPerson.Create("Bob");
        var lifePointToCreate = TestLifePointToCreate.Create(person);

        var result = lifePointToCreate.ToLifePoint(person);

        result.Id.Should().NotBeEmpty();
        result.Date.Should().Be(lifePointToCreate.Date);
        result.Caption.Should().Be(lifePointToCreate.Caption);
        result.Description.Should().Be(lifePointToCreate.Description);
        result.Latitude.Should().Be(lifePointToCreate.Latitude);
        result.Longitude.Should().Be(lifePointToCreate.Longitude);
        result.CreatedBy.Should().BeSameAs(person);
        result.ImageId.Should().BeNull();
    }

    [Test]
    public void LifePointToCreate_ToLifePoint_WithImage()
    {
        var person = TestPerson.Create("Bob");
        var imageId = Guid.NewGuid();
        var lifePointToCreate = TestLifePointToCreate.Create(person);

        var result = lifePointToCreate.ToLifePoint(person, imageId);

        result.ImageId.Should().Be(imageId);
    }

    [Test]
    public void ToExistingLocations_WithData()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() }.AsQueryable();

        var result = lifePoints.ToExistingLocations().ToList();

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(lifePoints.First().Id);
        result[1].Id.Should().Be(lifePoints.Last().Id);
    }

    [Test]
    public void ToExistingLocations_WithNull()
    {
        IQueryable<LifePoint>? source = null;

        var result = source.ToExistingLocations().ToList();

        result.Should().BeEmpty();
    }

    [Test]
    public void ToExistingPersons_WithData()
    {
        var persons = new[] { TestPerson.Create("Dixie"), TestPerson.Create("Ulf") }.AsQueryable();

        var result = persons.ToExistingPersons().ToList();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Dixie");
        result[1].Name.Should().Be("Ulf");
    }

    [Test]
    public void ToExistingPersons_WithNull()
    {
        IQueryable<Person>? source = null;

        var result = source.ToExistingPersons().ToList();

        result.Should().BeEmpty();
    }
}
