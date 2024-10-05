using System.Diagnostics.CodeAnalysis;
using DTO.LifePoint;
using Entities;
using Logging.Extensions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Persistence;

internal class ImageService(ILogger<ImageService> logger, IFileSystem fileSystem) : IImageService
{
    private static readonly string ImageDirectory = Path.Combine(Storage.UserDirectory, "images");

    /// <inheritdoc />
    public async Task<Guid> ProcessAndStoreImageAsync(Person owner, ImageToCreate newImage)
    {
        logger.MethodStarted();

        var imageId = Guid.NewGuid();
        var filePathForImage = GetFilePathForImage(owner, imageId);
        EnsureImagePathExists(filePathForImage);

        Image image;
        try
        {
            image = await Image.LoadAsync(newImage.Stream);
        }
        catch (Exception)
        {
            throw new NoImageException();
        }

        ResizeImage(image);

        await using var fileStream = fileSystem.CreateFile(filePathForImage);
        await image.SaveAsync(fileStream, new JpegEncoder());
        image.Dispose();
        logger.ImageResizedAndSaved(imageId);

        logger.MethodFinished();
        return imageId;
    }

    /// <inheritdoc />
    public Stream GetImage(Guid ownerId, Guid imageId)
    {
        logger.MethodStarted();
        return fileSystem.OpenRead(GetFilePathForImage(ownerId, imageId));
    }

    /// <inheritdoc />
    public void DeleteImage(Guid ownerId, Guid imageId)
    {
        logger.MethodStarted();
        fileSystem.DeleteFile(GetFilePathForImage(ownerId, imageId));
        logger.ImageDeleted(ownerId, imageId);
    }

    private static void ResizeImage(Image image) => image.Mutate(context => context.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(600) }));

    private static string GetFilePathForImage(Person owner, Guid imageId) => GetFilePathForImage(owner.Id, imageId);

    private static string GetFilePathForImage(Guid ownerId, Guid imageId) => Path.Combine(ImageDirectory, ownerId.ToString(), $"{imageId.ToString()}.jpg");

    [ExcludeFromCodeCoverage(Justification = "Found no way to tweak Directory.GetParent into returning null")]
    private static DirectoryInfo GetParentDirectory(string filePathForImage)
    {
        var parentDirectory = Directory.GetParent(filePathForImage) ??
                              throw new ArgumentNullException(nameof(filePathForImage), $"Could not resolve parent directory from {filePathForImage}");
        return parentDirectory;
    }

    private void EnsureImagePathExists(string filePathForImage)
    {
        var parentDirectory = GetParentDirectory(filePathForImage);
        if (!fileSystem.DirectoryExists(parentDirectory))
        {
            fileSystem.CreateDirectory(parentDirectory.ToString());
            logger.ImageDirectoryCreated(parentDirectory);
        }
    }
}