using AutoMapper;
using BusinessServices;
using BusinessServices.Services;
using DTO.LifePoint;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.Unit.BusinessServices;

[TestFixture]
[Category("Unit")]
public class LifePointServiceTests
{
    private readonly IStorage _storage = Substitute.For<IStorage>();
    private readonly IMapper _mapper = TestMapper.Create();

    [Test]
    public void GetAllLifePointLocations()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

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
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

        var results = testee.GetAllLocations(yearToFilter).ToList();

        results.Should().HaveCount(expectedNumberOfResults);
    }

    [Test]
    public void GetAllLifePointLocations_FilteredBy_Creator()
    {
        var person1 = TestPerson.Create("Dixie");
        var person2 = TestPerson.Create("Ulf");
        var lifePoints = new[] { TestLifePoint.Create(person1), TestLifePoint.Create(person2) };
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

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
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

        var results = testee.GetAllLocations(yearToFilter, useIdOfPerson1 ? person1.Id : Guid.Empty).ToList();

        results.Should().HaveCount(expectedNumberOfResults);
    }

    [Test]
    public void GetAllLifePointLocations_IsEmptyCollection_IfLifePointsInDbAreNull()
    {
        _storage.LifePoints.Returns((IQueryable<LifePoint>)null!);
        var testee = CreateTestee();

        var results = testee.GetAllLocations().ToList();

        results.Should().BeEmpty();
    }

    [Test]
    public void GetAllLifePointLocations_IsEmptyCollection_IfLifePointsInDbAreEmpty()
    {
        _storage.LifePoints.Returns(Array.Empty<LifePoint>().AsQueryable());
        var testee = CreateTestee();

        var results = testee.GetAllLocations().ToList();

        results.Should().BeEmpty();
    }

    [Test]
    public async Task GetLifePoint()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        _storage.FindAsync<LifePoint>(lifePoints[0].Id).Returns(lifePoints[0]);
        var testee = CreateTestee();

        var result = await testee.GetLifePointAsync(lifePoints[0].Id);

        result.Should().BeEquivalentTo(lifePoints[0], options => options.Excluding(x => x.CreatedBy));
        result.CreatedBy.Name.Should().Be(lifePoints[0].CreatedBy.Name);
    }

    [Test]
    public async Task GetLifePoint_ThrowsException_IfLifePointDoesNotExist()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        _storage.FindAsync<LifePoint>(lifePoints[0].Id).Returns((LifePoint?)null);
        var testee = CreateTestee();

        var testAction = () => testee.GetLifePointAsync(lifePoints[0].Id);

        await testAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task CreateNewLifePoint()
    {
        var person = new Person("Bob");
        var lifePointToCreate = TestLifePointToCreate.Create(person);
        _storage.FindAsync<Person>(lifePointToCreate.CreatedBy).Returns(person);
        _storage.AddItemAsync(Arg.Any<LifePoint>()).Returns(MapToLifePoint(lifePointToCreate, person));
        var testee = CreateTestee();

        var result = await testee.CreateLifePointAsync(lifePointToCreate);

        result.Id.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(lifePointToCreate, options => options.Excluding(x => x.CreatedBy).Excluding(x => x.ImageToCreate));
        result.CreatedBy.Name.Should().Be(person.Name);
        await _storage.Received(1).SaveAsync();
    }

    [Test]
    public async Task CreateNewLifePoint_WithImage()
    {
        var person = new Person("Bob");
        var newImage = new ImageToCreate(new MemoryStream(new byte[10]));
        var idOfCreatedImage = Guid.NewGuid();
        var lifePointToCreate = TestLifePointToCreate.Create(person, newImage);
        _storage.FindAsync<Person>(lifePointToCreate.CreatedBy).Returns(person);
        _storage.AddItemAsync(Arg.Any<LifePoint>()).Returns(MapToLifePoint(lifePointToCreate, person, idOfCreatedImage));
        _storage.StoreImageAsync(person, newImage).Returns(idOfCreatedImage);
        var testee = CreateTestee();

        var result = await testee.CreateLifePointAsync(lifePointToCreate);

        result.ImageId.Should().Be(idOfCreatedImage);
        await _storage.Received(1).SaveAsync();
        await _storage.Received(1).StoreImageAsync(person, lifePointToCreate.ImageToCreate!);
    }

    [Test]
    public async Task CreateNewLifePoint_ThrowsException_IfPersonDoesNotExist()
    {
        var lifePointToCreate = TestLifePointToCreate.Create();
        _storage.FindAsync<Person>(lifePointToCreate.CreatedBy).Returns((Person?)null);
        var testee = CreateTestee();

        var testAction = () => testee.CreateLifePointAsync(lifePointToCreate);

        await testAction.Should().ThrowAsync<ArgumentNullException>().WithMessage($"Could not find*{lifePointToCreate.CreatedBy}*");
    }

    [Test]
    public async Task DeleteLifePoint()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        _storage.FindAsync<LifePoint>(lifePoints[0].Id).Returns(lifePoints[0]);
        var testee = CreateTestee();

        await testee.DeleteLifePointAsync(lifePoints[0].Id);

        _storage.Received(1).RemoveItem(lifePoints[0]);
        await _storage.Received(1).SaveAsync();
    }

    [Test]
    public async Task DeleteLifePoint_IfImageExists()
    {
        var lifePoints = new[] { TestLifePoint.Create(imageId: Guid.NewGuid()), TestLifePoint.Create() };
        _storage.FindAsync<LifePoint>(lifePoints[0].Id).Returns(lifePoints[0]);
        var testee = CreateTestee();

        await testee.DeleteLifePointAsync(lifePoints[0].Id);

        _storage.Received(1).RemoveItem(lifePoints[0]);
        _storage.Received(1).DeleteImage(lifePoints[0].CreatedBy.Id, lifePoints[0].ImageId!.Value);
        await _storage.Received(1).SaveAsync();
    }

    [Test]
    public async Task DeleteLifePoint_ThrowsException_IfLifePointDoesNotExist()
    {
        var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
        _storage.FindAsync<LifePoint>(lifePoints[0].Id).Returns((LifePoint?)null);
        var testee = CreateTestee();

        var testAction = () => testee.DeleteLifePointAsync(lifePoints[0].Id);

        await testAction.Should().ThrowAsync<ArgumentNullException>().WithMessage($"Could not find*{lifePoints[0].Id}*");
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
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

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
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

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
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

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
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

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
        _storage.LifePoints.Returns(lifePoints.AsQueryable());
        var testee = CreateTestee();

        var results = testee.GetDistinctCreators(1951);

        results.Should().BeEmpty();
    }

    private LifePoint MapToLifePoint(LifePointToCreate lifePointToCreate, Person person, Guid? imageId = null)
    {
        return _mapper.Map<LifePointToCreate, LifePoint>(lifePointToCreate,
            options =>
            {
                options.Items[nameof(LifePoint.CreatedBy)] = person;
                options.Items[nameof(LifePoint.ImageId)] = imageId;
            });
    }

    private LifePointService CreateTestee() => new(Substitute.For<ILogger<LifePointService>>(), _storage, _mapper);
}