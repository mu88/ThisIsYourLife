using System.Net.Mime;
using BusinessServices;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebApp.Api;

namespace Tests.UnitTests.WebApp.Api;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Category("Unit")]
public class ImageControllerTests
{
    private readonly IStorage _storage = Substitute.For<IStorage>();

    [Test]
    public void GetImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        _storage.GetImage(ownerId, imageId).Returns(new MemoryStream());
        var testee = CreateTestee();

        var image = testee.GetImage(ownerId, imageId);

        image.Should().BeOfType<FileStreamResult>();
        image.As<FileStreamResult>().ContentType.Should().Be(MediaTypeNames.Image.Jpeg);
    }

    private ImageController CreateTestee() => new(_storage);
}