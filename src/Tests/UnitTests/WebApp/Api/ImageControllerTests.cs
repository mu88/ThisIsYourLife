using System;
using System.IO;
using System.Net.Mime;
using BusinessServices;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using WebApp.Api;

namespace Tests.UnitTests.WebApp.Api;

[TestFixture]
public class ImageControllerTests
{
    [Test]
    public void GetImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IStorage, Stream>(storage => storage.GetImage(ownerId, imageId)).Returns(new MemoryStream());
        var testee = autoMocker.CreateInstance<ImageController>();

        var image = testee.GetImage(ownerId, imageId);

        image.Should().BeOfType<FileStreamResult>();
        image.As<FileStreamResult>().ContentType.Should().Be(MediaTypeNames.Image.Jpeg);
    }
}