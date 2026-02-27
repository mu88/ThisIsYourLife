using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;

namespace Tests.Unit.Persistence;

[TestFixture]
[Category("Unit")]
public class StorageTests
{
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IImageService _imageService = Substitute.For<IImageService>();

    [Test]
    public void DeleteImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var testee = CreateTestee();

        testee.DeleteImage(ownerId, imageId);

        _imageService.Received(1).DeleteImage(ownerId, imageId);
    }

    [Test]
    public void OpenImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var testee = CreateTestee();

        testee.GetImage(ownerId, imageId);

        _imageService.Received(1).GetImage(ownerId, imageId);
    }

    [Test]
    public async Task StoreImage()
    {
        var person = TestPerson.Create("Dixie");
        var imageToCreate = TestImageToCreate.Create();
        var testee = CreateTestee();

        await testee.StoreImageAsync(person, imageToCreate);

        await _imageService.Received(1).ProcessAndStoreImageAsync(person, imageToCreate);
    }

    [Test]
    public async Task EnsureStorageExists_CreatesDirectory_IfItDoesNotExist()
    {
        _fileSystem.DirectoryExists(Storage.DatabaseDirectory).Returns(false);
        _fileSystem.FileExists(Storage.DatabasePath).Returns(true); // That's really only a hack to avoid further EF Core code
        var testee = CreateTestee();

        var testAction = () => testee.EnsureStorageExistsAsync();

        await testAction.Should().NotThrowAsync();
        _fileSystem.Received(1).CreateDirectory(Storage.DatabaseDirectory);
    }

    [Test]
    public async Task EnsureStorageExists_CreatesNothing_IfEverythingExists()
    {
        _fileSystem.DirectoryExists(Storage.DatabaseDirectory).Returns(true);
        _fileSystem.FileExists(Storage.DatabasePath).Returns(true);
        var testee = CreateTestee();

        var testAction = () => testee.EnsureStorageExistsAsync();

        await testAction.Should().NotThrowAsync();
        _fileSystem.DidNotReceive().CreateDirectory(Storage.DatabaseDirectory);
    }

    private Storage CreateTestee()
        => new(new DbContextOptionsBuilder<Storage>().Options,
            Substitute.For<ILogger<Storage>>(),
            _fileSystem,
            _imageService);
}
