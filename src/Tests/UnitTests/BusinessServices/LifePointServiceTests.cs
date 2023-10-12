using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessServices;
using BusinessServices.Services;
using DTO.LifePoint;
using Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.UnitTests.BusinessServices;

[TestFixture]
public class LifePointServiceTests
{
    [Test]
    public void GetAllLifePointLocations()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetAllLocations().ToList();

        results.Should().HaveCount(2);
        results.Should()
            .BeEquivalentTo(lifePoints,
                            options => options.Including(x => x.Id)
                                .Including(x => x.Latitude)
                                .Including(x => x.Longitude));
    }

    [TestCase(1953, 1953, 1)]
    [TestCase(1955, 1953, 0)]
    [TestCase(null, 1953, 2)]
    public void GetAllLifePointLocations_FilteredBy_Year(int? yearToFilter, int yearOfCreation, int expectedNumberOfResults)
    {
        var lifePoints = new[] { TestLifePoint.Create(date: new DateOnly(yearOfCreation, 4, 12)), TestLifePoint.Create(date: new DateOnly(1954, 4, 12)) };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetAllLocations(yearToFilter).ToList();

        results.Should().HaveCount(expectedNumberOfResults);
    }

    [Test]
    public void GetAllLifePointLocations_FilteredBy_Creator()
    {
        var person1 = TestPerson.Create("Dixie");
        var person2 = TestPerson.Create("Ulf");
        var lifePoints = new[] { TestLifePoint.Create(person1), TestLifePoint.Create(person2) };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetAllLocations(creatorId: person1.Id).ToList();

        results.Should().HaveCount(1);
    }

    [TestCase(1953, 1, true)]
    [TestCase(1953, 0, false)]
    [TestCase(1955, 0, true)]
    [TestCase(1955, 0, false)]
    [TestCase(null, 2, true)]
    [TestCase(null, 0, false)]
    public void GetAllLifePointLocations_FilteredBy_CreatorAndYear(int? yearToFilter, int expectedNumberOfResults, bool useIdOfPerson1)
    {
        var person1 = TestPerson.Create("Dixie");
        var person2 = TestPerson.Create("Ulf");
        var lifePoints = new[]
        {
            TestLifePoint.Create(person1, new DateOnly(1953, 4, 12)),
            TestLifePoint.Create(person1, new DateOnly(1954, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1953, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1954, 4, 12))
        };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetAllLocations(yearToFilter, useIdOfPerson1 ? person1.Id : Guid.Empty).ToList();

        results.Should().HaveCount(expectedNumberOfResults);
    }

    [Test]
    public void GetAllLifePointLocations_IsEmptyCollection_IfLifePointsInDbAreNull()
    {
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns((IQueryable<LifePoint>)null!);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetAllLocations().ToList();

        results.Should().BeEmpty();
    }

    [Test]
    public void GetAllLifePointLocations_IsEmptyCollection_IfLifePointsInDbAreEmpty()
    {
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(Array.Empty<LifePoint>().AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetAllLocations().ToList();

        results.Should().BeEmpty();
    }

    [Test]
    public async Task GetLifePoint()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<LifePoint?>>(x => x.FindAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync(lifePoints.First());
        var testee = autoMocker.CreateInstance<LifePointService>();

        var result = await testee.GetLifePointAsync(lifePoints.First().Id);

        result.Should().BeEquivalentTo(lifePoints.First(), options => options.Excluding(x => x.CreatedBy));
        result.CreatedBy.Name.Should().Be(lifePoints.First().CreatedBy.Name);
    }

    [Test]
    public async Task GetLifePoint_ThrowsException_IfLifePointDoesNotExist()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<LifePoint?>>(x => x.FindAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync((LifePoint?)null);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var testAction = () => testee.GetLifePointAsync(lifePoints.First().Id);

        await testAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task CreateNewLifePoint()
    {
        var person = new Person("Bob");
        var lifePointToCreate = TestLifePointToCreate.Create(person);
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<Person?>>(x => x.FindAsync<Person>(lifePointToCreate.CreatedBy)).ReturnsAsync(person);
        autoMocker.Setup<IStorage, Task<LifePoint>>(x => x.AddItemAsync(It.IsAny<LifePoint>())).Returns<LifePoint>(Task.FromResult);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var result = await testee.CreateLifePointAsync(lifePointToCreate);

        result.Id.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(lifePointToCreate, options => options.Excluding(x => x.CreatedBy).Excluding(x => x.ImageToCreate));
        result.CreatedBy.Name.Should().Be(person.Name);
        autoMocker.Verify<IStorage>(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task CreateNewLifePoint_WithImage()
    {
        var person = new Person("Bob");
        var newImage = new ImageToCreate(new MemoryStream(new byte[10]));
        var idOfCreatedImage = Guid.NewGuid();
        var lifePointToCreate = TestLifePointToCreate.Create(person, newImage);
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<Person?>>(x => x.FindAsync<Person>(lifePointToCreate.CreatedBy)).ReturnsAsync(person);
        autoMocker.Setup<IStorage, Task<LifePoint>>(x => x.AddItemAsync(It.IsAny<LifePoint>())).Returns<LifePoint>(Task.FromResult);
        autoMocker.Setup<IStorage, Task<Guid>>(x => x.StoreImageAsync(person, newImage)).ReturnsAsync(idOfCreatedImage);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var result = await testee.CreateLifePointAsync(lifePointToCreate);

        result.ImageId.Should().Be(idOfCreatedImage);
        autoMocker.Verify<IStorage>(x => x.SaveAsync(), Times.Once);
        autoMocker.Verify<IStorage>(x => x.StoreImageAsync(person, lifePointToCreate.ImageToCreate!), Times.Once);
    }

    [Test]
    public async Task CreateNewLifePoint_ThrowsException_IfPersonDoesNotExist()
    {
        var lifePointToCreate = TestLifePointToCreate.Create();
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<Person?>>(x => x.FindAsync<Person>(lifePointToCreate.CreatedBy)).ReturnsAsync((Person?)null);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var testAction = () => testee.CreateLifePointAsync(lifePointToCreate);

        await testAction.Should().ThrowAsync<ArgumentNullException>().WithMessage($"Could not find*{lifePointToCreate.CreatedBy}*");
    }

    [Test]
    public async Task DeleteLifePoint()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<LifePoint?>>(x => x.FindAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync(lifePoints.First());
        var testee = autoMocker.CreateInstance<LifePointService>();

        await testee.DeleteLifePointAsync(lifePoints.First().Id);

        autoMocker.Verify<IStorage>(x => x.RemoveItem(lifePoints.First()), Times.Once);
        autoMocker.Verify<IStorage>(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteLifePoint_IfImageExists()
    {
        var lifePoints = new[] { TestLifePoint.Create(imageId: Guid.NewGuid()), TestLifePoint.Create() };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<LifePoint?>>(x => x.FindAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync(lifePoints.First());
        var testee = autoMocker.CreateInstance<LifePointService>();

        await testee.DeleteLifePointAsync(lifePoints.First().Id);

        autoMocker.Verify<IStorage>(x => x.RemoveItem(lifePoints.First()), Times.Once);
        autoMocker.Verify<IStorage>(x => x.DeleteImage(lifePoints.First().CreatedBy.Id, lifePoints.First().ImageId!.Value), Times.Once);
        autoMocker.Verify<IStorage>(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteLifePoint_ThrowsException_IfLifePointDoesNotExist()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Task<LifePoint?>>(x => x.FindAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync((LifePoint?)null);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var testAction = () => testee.DeleteLifePointAsync(lifePoints.First().Id);

        await testAction.Should().ThrowAsync<ArgumentNullException>().WithMessage($"Could not find*{lifePoints.First().Id}*");
    }

    [Test]
    public void GetDistinctYears()
    {
        var lifePoints = new[]
        {
            TestLifePoint.Create(date: new DateOnly(1953, 4, 12)),
            TestLifePoint.Create(date: new DateOnly(1952, 4, 12)),
            TestLifePoint.Create(date: new DateOnly(1952, 4, 12))
        };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetDistinctYears();

        results.Should().Equal(1952, 1953);
    }

    [Test]
    public void GetDistinctYears_FilteredBy_Creator()
    {
        var person1 = TestPerson.Create("Dixie");
        var person2 = TestPerson.Create("Ulf");
        var lifePoints = new[]
        {
            TestLifePoint.Create(person1, new DateOnly(1953, 4, 12)),
            TestLifePoint.Create(person1, new DateOnly(1952, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1952, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1951, 4, 12))
        };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetDistinctYears(person2.Id);

        results.Should().Equal(1951, 1952);
    }

    [Test]
    public void GetDistinctCreatorNames()
    {
        var person1 = TestPerson.Create("Dixie");
        var person2 = TestPerson.Create("Ulf");
        var lifePoints = new[]
        {
            TestLifePoint.Create(person1, new DateOnly(1953, 4, 12)),
            TestLifePoint.Create(person1, new DateOnly(1952, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1952, 4, 12))
        };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetDistinctCreators();

        results.Select(x => x.Name).Should().Equal("Dixie", "Ulf");
    }

    [Test]
    public void GetDistinctCreatorNames_FilteredBy_Years()
    {
        var person1 = TestPerson.Create("Dixie");
        var person2 = TestPerson.Create("Ulf");
        var lifePoints = new[]
        {
            TestLifePoint.Create(person1, new DateOnly(1953, 4, 12)),
            TestLifePoint.Create(person1, new DateOnly(1952, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1952, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1953, 4, 12))
        };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetDistinctCreators(1953);

        results.Select(x => x.Name).Should().Equal("Dixie", "Ulf");
    }
    
    [Test]
    public void GetDistinctCreatorNames_FilteredBy_NotMatchingYear()
    {
        var person1 = TestPerson.Create("Dixie");
        var person2 = TestPerson.Create("Ulf");
        var lifePoints = new[]
        {
            TestLifePoint.Create(person1, new DateOnly(1953, 4, 12)),
            TestLifePoint.Create(person1, new DateOnly(1952, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1952, 4, 12)),
            TestLifePoint.Create(person2, new DateOnly(1953, 4, 12))
        };
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, IQueryable<LifePoint>>(x => x.LifePoints).Returns(lifePoints.AsQueryable);
        var testee = autoMocker.CreateInstance<LifePointService>();

        var results = testee.GetDistinctCreators(1951);

        results.Should().BeEmpty();
    }
}