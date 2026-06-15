using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using Persistence;
using SkiaSharp;
using Tests.Doubles;

namespace Tests.Unit.Persistence;

[TestFixture]
[Category("Unit")]
public class ImageServiceTests
{
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();

    [Test]
    public void GetImage()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var testee = CreateTestee();

        // Act
        testee.GetImage(ownerId, imageId);

        // Assert
        _fileSystem.Received(1).OpenRead(Arg.Is<string>(s => s.Contains(ownerId.ToString()) && s.Contains(imageId.ToString())));
    }

    [Test]
    public void DeleteImage()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var testee = CreateTestee();

        // Act
        testee.DeleteImage(ownerId, imageId);

        // Assert
        _fileSystem.Received(1).DeleteFile(Arg.Is<string>(s => s.Contains(ownerId.ToString()) && s.Contains(imageId.ToString())));
    }

    [Test]
    public async Task ProcessAndStoreImage()
    {
        // Arrange
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.Create();
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new MemoryStream());
        _fileSystem.DirectoryExists(Arg.Any<DirectoryInfo>()).Returns(true);
        var testee = CreateTestee();

        // Act
        var result = await testee.ProcessAndStoreImageAsync(person, newImage);

        // Assert
        result.Should().NotBeEmpty();
        _fileSystem.Received(1).CreateFile(Arg.Is<string>(s => s.Contains(person.Id.ToString())));
    }

    [Test]
    public async Task ProcessAndStoreImage_ShouldFail_IfInputIsNoImage()
    {
        // Arrange
        var person = TestPerson.Create("Dixie");
        var notAnImageStream = new MemoryStream(new byte[10]);
        var newImage = TestImageToCreate.Create(notAnImageStream);
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new MemoryStream());
        _fileSystem.DirectoryExists(Arg.Any<DirectoryInfo>()).Returns(true);
        var testee = CreateTestee();

        // Act
        var testAction = async () => await testee.ProcessAndStoreImageAsync(person, newImage);

        // Assert
        await testAction.Should().ThrowAsync<NoImageException>();
    }

    [Test]
    public async Task ProcessAndStoreImage_CreatesImageDirectory()
    {
        // Arrange
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.Create();
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new MemoryStream());
        _fileSystem.DirectoryExists(Arg.Any<DirectoryInfo>()).Returns(false);
        var testee = CreateTestee();

        // Act
        var result = await testee.ProcessAndStoreImageAsync(person, newImage);

        // Assert
        result.Should().NotBeEmpty();
        _fileSystem.Received(1).CreateDirectory(Arg.Is<string>(s => s.Contains(person.Id.ToString())));
    }

    [Test]
    public async Task ProcessAndStoreImage_OutputIsValidJpeg()
    {
        // Arrange
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.Create();
        var outputStream = new MemoryStream();
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new NonDisposingStream(outputStream));
        _fileSystem.DirectoryExists(Arg.Any<DirectoryInfo>()).Returns(true);
        var testee = CreateTestee();

        // Act
        await testee.ProcessAndStoreImageAsync(person, newImage);

        // Assert
        outputStream.Position = 0;
        using var decodedBitmap = SKBitmap.Decode(outputStream);
        decodedBitmap.Should().NotBeNull();
    }

    [Test]
    public async Task ProcessAndStoreImage_LargeImage_IsResizedToMax600px()
    {
        // Arrange
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.CreateWithSize(1200, 800);
        var outputStream = new MemoryStream();
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new NonDisposingStream(outputStream));
        _fileSystem.DirectoryExists(Arg.Any<DirectoryInfo>()).Returns(true);
        var testee = CreateTestee();

        // Act
        await testee.ProcessAndStoreImageAsync(person, newImage);

        // Assert
        outputStream.Position = 0;
        using var decodedBitmap = SKBitmap.Decode(outputStream);
        decodedBitmap.Should().NotBeNull();
        Math.Max(decodedBitmap!.Width, decodedBitmap.Height).Should().BeLessThanOrEqualTo(600);
    }

    [Test]
    public async Task ProcessAndStoreImage_LargeImage_PreservesAspectRatio()
    {
        // Arrange
        var person = TestPerson.Create("Dixie");
        var newImage = TestImageToCreate.CreateWithSize(1200, 800);
        var outputStream = new MemoryStream();
        _fileSystem.CreateFile(Arg.Any<string>()).Returns(new NonDisposingStream(outputStream));
        _fileSystem.DirectoryExists(Arg.Any<DirectoryInfo>()).Returns(true);
        var testee = CreateTestee();

        // Act
        await testee.ProcessAndStoreImageAsync(person, newImage);

        // Assert
        outputStream.Position = 0;
        using var decodedBitmap = SKBitmap.Decode(outputStream);
        decodedBitmap.Should().NotBeNull();
        var expectedRatio = 1200f / 800f;
        var actualRatio = decodedBitmap!.Width / (float)decodedBitmap.Height;
        actualRatio.Should().BeApproximately(expectedRatio, 0.01f);
    }

    private ImageService CreateTestee() => new(Options.Create(new StorageOptions()), Substitute.For<ILogger<ImageService>>(), _fileSystem);

    private sealed class NonDisposingStream(Stream innerStream) : Stream
    {
        public override bool CanRead => innerStream.CanRead;

        public override bool CanSeek => innerStream.CanSeek;

        public override bool CanWrite => innerStream.CanWrite;

        public override long Length => innerStream.Length;

        public override long Position
        {
            get => innerStream.Position;
            set => innerStream.Position = value;
        }

        public override void Flush() => innerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => innerStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => innerStream.Seek(offset, origin);

        public override void SetLength(long value) => innerStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => innerStream.Write(buffer, offset, count);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            innerStream.WriteAsync(buffer, offset, count, cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                innerStream.Flush();
            }

            base.Dispose(disposing);
        }
    }
}
