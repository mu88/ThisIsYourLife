using System;
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
    public void DeleteImage()
    {
        var imageId = Guid.NewGuid();
        var autoMocker = CreateAutoMocker();
        var testee = autoMocker.CreateInstance<Storage>();

        testee.DeleteImage(imageId);

        autoMocker.Verify<IFileSystem>(system => system.DeleteFile(It.Is<string>(s => s.Contains(imageId.ToString()))), Times.Once);
    }

    [Test]
    public void OpenImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var autoMocker = CreateAutoMocker();
        var testee = autoMocker.CreateInstance<Storage>();

        testee.GetImage(ownerId, imageId);

        autoMocker.Verify<IFileSystem>(system => system.OpenRead(It.Is<string>(s => s.Contains(ownerId.ToString()) && s.Contains(imageId.ToString()))), Times.Once);
    }

    [Test]
    public void EnsureStorageExists_CreatesDirectory_IfItDoesNotExist()
    {
        var autoMocker = CreateAutoMocker();
        autoMocker.Setup<IFileSystem, bool>(system => system.DirectoryExists(Storage.DatabaseDirectory)).Returns(false);
        autoMocker.Setup<IFileSystem, bool>(system => system.FileExists(Storage.DatabasePath)).Returns(true); // That's really only a hack to avoid further EF Core code
        var testee = autoMocker.CreateInstance<Storage>();

        var testAction = () => testee.EnsureStorageExistsAsync();

        testAction.Should().NotThrowAsync();
        autoMocker.Verify<IFileSystem>(system => system.CreateDirectory(Storage.DatabaseDirectory), Times.Once);
    }

    [Test]
    public void EnsureStorageExists_CreatesNothing_IfEverythingExists()
    {
        var autoMocker = CreateAutoMocker();
        autoMocker.Setup<IFileSystem, bool>(system => system.DirectoryExists(Storage.DatabaseDirectory)).Returns(true);
        autoMocker.Setup<IFileSystem, bool>(system => system.FileExists(Storage.DatabasePath)).Returns(true);
        var testee = autoMocker.CreateInstance<Storage>();

        var testAction = () => testee.EnsureStorageExistsAsync();

        testAction.Should().NotThrowAsync();
        autoMocker.Verify<IFileSystem>(system => system.CreateDirectory(Storage.DatabaseDirectory), Times.Never);
    }

    private static CustomAutoMocker CreateAutoMocker()
    {
        var autoMocker = new CustomAutoMocker();
        autoMocker.Use(new DbContextOptionsBuilder<Storage>().Options);
        return autoMocker;
    }
}