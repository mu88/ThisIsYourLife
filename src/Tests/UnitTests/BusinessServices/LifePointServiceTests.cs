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
            autoMocker.Setup<IStorage, Task<LifePoint?>>(x => x.FindAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync(lifePoints.First());
            var testee = autoMocker.CreateInstance<LifePointService>();

            var result = await testee.GetLifePointAsync(lifePoints.First().Id);

            result.Should().BeEquivalentTo(lifePoints.First(), options => options.Excluding(x => x.CreatedBy));
            result.CreatedBy.Should().Be(lifePoints.First().CreatedBy.Name);
        }

        [Test]
        public async Task GetLifePoint_ThrowsException_IfLifePointDoesNotExist()
        {
            var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
            var autoMocker = new CustomAutoMocker();
            autoMocker.Setup<IStorage, Task<LifePoint?>>(x => x.FindAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync((LifePoint?)null);
            var testee = autoMocker.CreateInstance<LifePointService>();

            Func<Task<ExistingLifePoint>> testAction = async () => await testee.GetLifePointAsync(lifePoints.First().Id);

            await testAction.Should().ThrowAsync<NullReferenceException>();
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
            result.Should().BeEquivalentTo(lifePointToCreate, options => options.Excluding(x => x.CreatedBy).Excluding(x => x.ImageStream));
            result.CreatedBy.Should().Be(person.Name);
            autoMocker.Verify<IStorage>(x => x.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task CreateNewLifePoint_WithImage()
        {
            var person = new Person("Bob");
            var image = new MemoryStream(new byte[10]);
            var idOfCreatedImage = Guid.NewGuid();
            var lifePointToCreate = TestLifePointToCreate.Create(person, image);
            var autoMocker = new CustomAutoMocker();
            autoMocker.Setup<IStorage, Task<Person?>>(x => x.FindAsync<Person>(lifePointToCreate.CreatedBy)).ReturnsAsync(person);
            autoMocker.Setup<IStorage, Task<LifePoint>>(x => x.AddItemAsync(It.IsAny<LifePoint>())).Returns<LifePoint>(Task.FromResult);
            autoMocker.Setup<IStorage, Task<Guid>>(x => x.StoreImageAsync(image)).ReturnsAsync(idOfCreatedImage);
            var testee = autoMocker.CreateInstance<LifePointService>();

            var result = await testee.CreateLifePointAsync(lifePointToCreate);

            result.ImageId.Should().Be(idOfCreatedImage);
            autoMocker.Verify<IStorage>(x => x.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task CreateNewLifePoint_ThrowsException_IfPersonDoesNotExist()
        {
            var lifePointToCreate = TestLifePointToCreate.Create();
            var autoMocker = new CustomAutoMocker();
            autoMocker.Setup<IStorage, Task<Person?>>(x => x.FindAsync<Person>(lifePointToCreate.CreatedBy)).ReturnsAsync((Person?)null);
            var testee = autoMocker.CreateInstance<LifePointService>();

            Func<Task<ExistingLifePoint>> testAction = async () => await testee.CreateLifePointAsync(lifePointToCreate);

            await testAction.Should().ThrowAsync<NullReferenceException>();
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
        public async Task DeleteLifePoint_ThrowsException_IfLifePointDoesNotExist()
        {
            var lifePoints = new[] { TestLifePoint.Create(), TestLifePoint.Create() };
            var autoMocker = new CustomAutoMocker();
            autoMocker.Setup<IStorage, Task<LifePoint?>>(x => x.FindAsync<LifePoint>(lifePoints.First().Id)).ReturnsAsync((LifePoint?)null);
            var testee = autoMocker.CreateInstance<LifePointService>();

            Func<Task> testAction = async () => await testee.DeleteLifePointAsync(lifePoints.First().Id);

            await testAction.Should().ThrowAsync<NullReferenceException>();
        }
    }
}