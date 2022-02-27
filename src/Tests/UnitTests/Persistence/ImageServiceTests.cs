using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;

namespace Tests.UnitTests.Persistence;

[TestFixture]
public class ImageServiceTests
{
    [Test]
    public void GetImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var autoMocker = new CustomAutoMocker();
        var testee = autoMocker.CreateInstance<ImageService>();

        testee.GetImage(ownerId, imageId);

        autoMocker.Verify<IFileSystem>(system => system.OpenRead(It.Is<string>(s => s.Contains(ownerId.ToString()) && s.Contains(imageId.ToString()))), Times.Once);
    }

    [Test]
    public void DeleteImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var autoMocker = new CustomAutoMocker();
        var testee = autoMocker.CreateInstance<ImageService>();

        testee.DeleteImage(ownerId, imageId);

        autoMocker.Verify<IFileSystem>(system => system.DeleteFile(It.Is<string>(s => s.Contains(ownerId.ToString()) && s.Contains(imageId.ToString()))), Times.Once);
    }

    [Test]
    public async Task ProcessAndStoreImage()
    {
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.Create();
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IFileSystem, Stream>(system => system.CreateFile(It.IsAny<string>())).Returns(new MemoryStream());
        autoMocker.Setup<IFileSystem, bool>(system => system.DirectoryExists(It.IsAny<string>())).Returns(true);
        var testee = autoMocker.CreateInstance<ImageService>();

        var result = await testee.ProcessAndStoreImageAsync(person, newImage);

        result.Should().NotBeEmpty();
        autoMocker.Verify<IFileSystem>(system => system.CreateFile(It.Is<string>(s => s.Contains(person.Id.ToString()))));
    }

    [Test]
    public async Task ProcessAndStoreImage_CreatesImageDirectory()
    {
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.Create();
        var autoMocker = new CustomAutoMocker();
        autoMocker.Setup<IFileSystem, Stream>(system => system.CreateFile(It.IsAny<string>())).Returns(new MemoryStream());
        autoMocker.Setup<IFileSystem, bool>(system => system.DirectoryExists(It.IsAny<string>())).Returns(false);
        var testee = autoMocker.CreateInstance<ImageService>();

        var result = await testee.ProcessAndStoreImageAsync(person, newImage);

        result.Should().NotBeEmpty();
        autoMocker.Verify<IFileSystem>(system => system.CreateDirectory(It.Is<string>(s => s.Contains(person.Id.ToString()))));
    }
}