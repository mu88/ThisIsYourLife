using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessServices;
using BusinessServices.Services;
using Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.UnitTests.BusinessServices
{
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
            autoMocker.Setup<IStorage, Task<LifePoint>>(x => x.GetAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync(lifePoints.First());
            var testee = autoMocker.CreateInstance<LifePointService>();

            var result = await testee.GetLifePointAsync(lifePoints.First().Id);

            result.Should().BeEquivalentTo(lifePoints.First(), options => options.Excluding(x => x.CreatedBy));
            result.CreatedBy.Should().Be(lifePoints.First().CreatedBy.Name);
        }

        [Test]
        public async Task CreateNewLifePoint()
        {
            var person = new Person("Bob");
            var lifePointToCreate = TestLifePointToCreate.Create(person);
            var autoMocker = new CustomAutoMocker();
            autoMocker.Setup<IStorage, Task<Person>>(x => x.GetAsync<Person>(lifePointToCreate.CreatedBy)).ReturnsAsync(person);
            autoMocker.Setup<IStorage, Task<LifePoint>>(x => x.AddItemAsync(It.IsAny<LifePoint>())).Returns<LifePoint>(x => Task.FromResult(x));
            var testee = autoMocker.CreateInstance<LifePointService>();

            var result = await testee.CreateLifePointAsync(lifePointToCreate);

            result.Id.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(lifePointToCreate, options => options.Excluding(x => x.CreatedBy));
            result.CreatedBy.Should().Be(person.Name);
            autoMocker.Verify<IStorage>(x => x.SaveAsync(), Times.Once);
        }
    }
}