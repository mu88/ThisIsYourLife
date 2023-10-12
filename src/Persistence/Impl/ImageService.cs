using System;
using System.IO;
using System.Threading.Tasks;
using DTO.LifePoint;
using Entities;
using Logging.Extensions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Persistence;

internal class ImageService : IImageService
{
    private static readonly string ImageDirectory = Path.Combine(Storage.UserDirectory, "images");

    private readonly ILogger<ImageService> _logger;
    private readonly IFileSystem _fileSystem;

    public ImageService(ILogger<ImageService> logger, IFileSystem fileSystem)
    {
        _logger = logger;
        _fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public async Task<Guid> ProcessAndStoreImageAsync(Person owner, ImageToCreate newImage)
    {
        _logger.MethodStarted();

        var imageId = Guid.NewGuid();
        var filePathForImage = GetFilePathForImage(owner, imageId);
        EnsureImagePathExists(filePathForImage);

        Image image;
        try { image = await Image.LoadAsync(newImage.Stream); }
        catch (Exception) { throw new NoImageException(); }

        ResizeImage(image);

        await using var fileStream = _fileSystem.CreateFile(filePathForImage);
        await image.SaveAsync(fileStream, new JpegEncoder());
        image.Dispose();
        _logger.ImageResizedAndSaved(imageId);

        _logger.MethodFinished();
        return imageId;
    }

    /// <inheritdoc />
    public Stream GetImage(Guid ownerId, Guid imageId)
    {
        _logger.MethodStarted();
        return _fileSystem.OpenRead(GetFilePathForImage(ownerId, imageId));
    }

    /// <inheritdoc />
    public void DeleteImage(Guid ownerId, Guid imageId)
    {
        _logger.MethodStarted();
        _fileSystem.DeleteFile(GetFilePathForImage(ownerId, imageId));
        _logger.ImageDeleted(ownerId, imageId);
    }

    private static void ResizeImage(Image image) => image.Mutate(context => context.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(600) }));

    private void EnsureImagePathExists(string filePathForImage)
    {
        var parentDirectory = Directory.GetParent(filePathForImage) ?? throw new ArgumentNullException(nameof(filePathForImage), $"Could not resolve parent directory from {filePathForImage}");
        if (!_fileSystem.DirectoryExists(parentDirectory))
        {
            _fileSystem.CreateDirectory(parentDirectory.ToString());
            _logger.ImageDirectoryCreated(parentDirectory);
        }
    }

    private static string GetFilePathForImage(Person owner, Guid imageId) => GetFilePathForImage(owner.Id, imageId);

    private static string GetFilePathForImage(Guid ownerId, Guid imageId) => Path.Combine(ImageDirectory, ownerId.ToString(), $"{imageId.ToString()}.jpg");
}