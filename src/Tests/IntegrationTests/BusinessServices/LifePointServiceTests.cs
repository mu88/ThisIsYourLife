using System.Linq;
using System.Threading.Tasks;
using BusinessServices.Services;
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
    }
}