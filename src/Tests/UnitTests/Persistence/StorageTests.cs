using System;
using System.IO;
using System.Threading.Tasks;
using DTO.LifePoint;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Persistence;

namespace Tests.UnitTests.Persistence;

[TestFixture]
public class StorageTests
{
    [Test]
    public async Task StoreImage()
    {
        var imageStream = new MemoryStream(new byte[10]);
        var newImage = new ImageToCreate(imageStream);
        var fileSystemMock = new Mock<IFileSystem>();
        var testee = new Storage(new DbContextOptionsBuilder<Storage>().Options, fileSystemMock.Object);

        var result = await testee.StoreImageAsync(newImage);

        result.Should().NotBeEmpty();
        fileSystemMock.Verify(x => x.CreateFileAsync(It.IsAny<string>(), imageStream), Times.Once);
    }

    [Test]
    public void DeleteImage()
    {
        var imageId = Guid.NewGuid();
        var fileSystemMock = new Mock<IFileSystem>();
        var testee = new Storage(new DbContextOptionsBuilder<Storage>().Options, fileSystemMock.Object);

        testee.DeleteImage(imageId);

        fileSystemMock.Verify(system => system.DeleteFile(It.Is<string>(s => s.Contains(imageId.ToString()))), Times.Once);
    }
}