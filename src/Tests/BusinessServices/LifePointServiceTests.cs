using System.Linq;
using BusinessServices;
using BusinessServices.Services;
using Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.BusinessServices
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
    }
}