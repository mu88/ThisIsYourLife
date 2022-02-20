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
        var newImage = new ImageToCreate("bla.png", imageStream);
        var fileSystemMock = new Mock<IFileSystem>();
        var testee = new Storage(new DbContextOptionsBuilder<Storage>().Options, fileSystemMock.Object);

        var result = await testee.StoreImageAsync(newImage);

        result.Should().NotBeEmpty();
        fileSystemMock.Verify(x => x.CreateFileAsync(It.Is<string>(s => s.Contains(".png")), imageStream), Times.Once);
    }
}