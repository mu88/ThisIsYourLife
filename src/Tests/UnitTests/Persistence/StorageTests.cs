using System;
using System.IO;
using System.Threading.Tasks;
using DTO.LifePoint;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;

namespace Tests.UnitTests.Persistence;

[TestFixture]
public class StorageTests
{
    [Test]
    public async Task StoreImage()
    {
        var person = TestPerson.Create("Bob");
        var imageStream = new MemoryStream(new byte[10]);
        var newImage = new ImageToCreate(imageStream);
        var fileSystemMock = new Mock<IFileSystem>();
        var testee = new Storage(new DbContextOptionsBuilder<Storage>().Options, fileSystemMock.Object);

        var result = await testee.StoreImageAsync(person, newImage);

        result.Should().NotBeEmpty();
        fileSystemMock.Verify(x => x.CreateFileAsync(It.Is<string>(s => s.Contains(person.Id.ToString())), imageStream), Times.Once);
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

    [Test]
    public void OpenImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var fileSystemMock = new Mock<IFileSystem>();
        var testee = new Storage(new DbContextOptionsBuilder<Storage>().Options, fileSystemMock.Object);

        testee.GetImage(ownerId, imageId);

        fileSystemMock.Verify(system => system.OpenRead(It.Is<string>(s => s.Contains(ownerId.ToString()) && s.Contains(imageId.ToString()))), Times.Once);
    }
}