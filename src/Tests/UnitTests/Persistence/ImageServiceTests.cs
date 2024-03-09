using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;

namespace Tests.UnitTests.Persistence;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Category("Unit")]
public class ImageServiceTests
{
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();

    [Test]
    public void GetImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var testee = CreateTestee();

        testee.GetImage(ownerId, imageId);

        _fileSystem.Received(1).OpenRead(Arg.Is<string>(s => s.Contains(ownerId.ToString()) && s.Contains(imageId.ToString())));
    }

    [Test]
    public void DeleteImage()
    {
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var testee = CreateTestee();

        testee.DeleteImage(ownerId, imageId);

        _fileSystem.Received(1).DeleteFile(Arg.Is<string>(s => s.Contains(ownerId.ToString()) && s.Contains(imageId.ToString())));
    }

    [Test]
    public async Task ProcessAndStoreImage()
    {
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.Create();
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new MemoryStream());
        _fileSystem.DirectoryExists(Arg.Any<string>()).Returns(true);
        var testee = CreateTestee();

        var result = await testee.ProcessAndStoreImageAsync(person, newImage);

        result.Should().NotBeEmpty();
        _fileSystem.Received(1).CreateFile(Arg.Is<string>(s => s.Contains(person.Id.ToString())));
    }

    [Test]
    public async Task ProcessAndStoreImage_ShouldFail_IfInputIsNoImage()
    {
        var person = TestPerson.Create("Dixie");
        var notAnImageStream = new MemoryStream(new byte[10]);
        var newImage = TestImageToCreate.Create(notAnImageStream);
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new MemoryStream());
        _fileSystem.DirectoryExists(Arg.Any<string>()).Returns(true);
        var testee = CreateTestee();

        var testAction = async () => await testee.ProcessAndStoreImageAsync(person, newImage);

        await testAction.Should().ThrowAsync<NoImageException>();
    }

    [Test]
    public async Task ProcessAndStoreImage_CreatesImageDirectory()
    {
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.Create();
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new MemoryStream());
        _fileSystem.DirectoryExists(Arg.Any<string>()).Returns(false);
        var testee = CreateTestee();

        var result = await testee.ProcessAndStoreImageAsync(person, newImage);

        result.Should().NotBeEmpty();
        _fileSystem.Received(1).CreateDirectory(Arg.Is<string>(s => s.Contains(person.Id.ToString())));
    }

    private ImageService CreateTestee() => new(Substitute.For<ILogger<ImageService>>(), _fileSystem);
}