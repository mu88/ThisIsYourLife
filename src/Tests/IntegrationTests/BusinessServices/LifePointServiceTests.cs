using System.Linq;
using System.Threading.Tasks;
using BusinessServices.Services;
using Entities;
using FluentAssertions;
using NUnit.Framework;
using Tests.Doubles;

namespace Tests.IntegrationTests.BusinessServices
{
    [TestFixture]
    public class LifePointServiceTests
    {
        [Test]
        public async Task GetAllLocations()
        {
            var storage = TestStorage.Create();
            await storage.AddItemAsync(TestLifePoint.Create());
            await storage.AddItemAsync(TestLifePoint.Create());
            await storage.SaveAsync();
            var testee = new LifePointService(storage, TestMapper.Create());

            var results = testee.GetAllLocations().ToList();

            results.Should().HaveCount(2);
            results.Should()
                .BeEquivalentTo(storage.LifePoints,
                                options => options.Including(x => x.Id)
                                    .Including(x => x.Latitude)
                                    .Including(x => x.Longitude));
        }

        [Test]
        public async Task GetLifePoint()
        {
            var lifePoint = TestLifePoint.Create();
            var storage = TestStorage.Create();
            await storage.AddItemAsync(lifePoint);
            await storage.SaveAsync();
            var testee = new LifePointService(storage, TestMapper.Create());

            var result = await testee.GetLifePointAsync(lifePoint.Id);

            result.Should().BeEquivalentTo(lifePoint, options => options.Excluding(x => x.CreatedBy));
            result.CreatedBy.Should().Be(lifePoint.CreatedBy.Name);
        }

        [Test]
        public async Task CreateNewLifePoint()
        {
            var storage = TestStorage.Create();
            var person = await storage.AddItemAsync(new Person("Bob"));
            var lifePointToCreate = TestLifePointToCreate.Create(person);
            var testee = new LifePointService(storage, TestMapper.Create());

            var result = await testee.CreateLifePointAsync(lifePointToCreate);

            storage.LifePoints.Should().ContainSingle(x => x.Id == result.Id);
        }

        [Test]
        public async Task DeleteLifePoint()
        {
            var lifePoint = TestLifePoint.Create();
            var storage = TestStorage.Create();
            await storage.AddItemAsync(lifePoint);
            await storage.SaveAsync();
            var testee = new LifePointService(storage, TestMapper.Create());

            await testee.DeleteLifePointAsync(lifePoint.Id);

            storage.LifePoints.Should().NotContain(lifePoint);
        }
    }
}