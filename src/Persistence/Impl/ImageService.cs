using System.Diagnostics.CodeAnalysis;
using DTO.LifePoint;
using Entities;
using Logging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace Persistence;

internal class ImageService(IOptions<StorageOptions> storageOptions, ILogger<ImageService> logger, IFileSystem fileSystem) : IImageService
{
    private string ImageDirectory => Path.Combine(storageOptions.Value.BasePath, "images");

    /// <inheritdoc />
    public async Task<Guid> ProcessAndStoreImageAsync(Person owner, ImageToCreate newImage)
    {
        using var activity = Tracing.Source.StartActivity();

        var imageId = Guid.NewGuid();
        var filePathForImage = GetFilePathForImage(owner, imageId);
        EnsureImagePathExists(filePathForImage);

        using var image = SKBitmap.Decode(newImage.Stream) ?? throw new NoImageException();
        using var resizedImage = ResizeImage(image);
        using var skImage = SKImage.FromBitmap(resizedImage);
        using var encodedData = skImage.Encode(SKEncodedImageFormat.Jpeg, 90);
        await using var fileStream = fileSystem.CreateFile(filePathForImage);
        var encodedBytes = encodedData.ToArray();
        await fileStream.WriteAsync(encodedBytes, 0, encodedBytes.Length);
        logger.ImageResizedAndSaved(imageId);

        return imageId;
    }

    /// <inheritdoc />
    public Stream GetImage(Guid ownerId, Guid imageId)
    {
        using var activity = Tracing.Source.StartActivity();
        return fileSystem.OpenRead(GetFilePathForImage(ownerId, imageId));
    }

    /// <inheritdoc />
    public void DeleteImage(Guid ownerId, Guid imageId)
    {
        using var activity = Tracing.Source.StartActivity();
        fileSystem.DeleteFile(GetFilePathForImage(ownerId, imageId));
        logger.ImageDeleted(ownerId, imageId);
    }

    private static SKBitmap ResizeImage(SKBitmap source)
    {
        const int maxSizeInPixels = 600;

        if (source.Width <= maxSizeInPixels && source.Height <= maxSizeInPixels)
        {
            return source.Copy();
        }

        var resizeFactor = maxSizeInPixels / (float)Math.Max(source.Width, source.Height);
        var targetWidth = Math.Max(1, (int)Math.Round(source.Width * resizeFactor));
        var targetHeight = Math.Max(1, (int)Math.Round(source.Height * resizeFactor));
        var targetInfo = new SKImageInfo(targetWidth, targetHeight, source.ColorType, source.AlphaType);

        return source.Resize(targetInfo, SKSamplingOptions.Default) ?? source.Copy();
    }

    [ExcludeFromCodeCoverage(Justification = "Found no way to tweak Directory.GetParent into returning null")]
    private static DirectoryInfo GetParentDirectory(string filePathForImage)
    {
        var parentDirectory = Directory.GetParent(filePathForImage) ??
            throw new ArgumentNullException(nameof(filePathForImage), $"Could not resolve parent directory from {filePathForImage}");
        return parentDirectory;
    }

    private string GetFilePathForImage(Person owner, Guid imageId) => GetFilePathForImage(owner.Id, imageId);

    private string GetFilePathForImage(Guid ownerId, Guid imageId) => Path.Combine(ImageDirectory, ownerId.ToString(), $"{imageId.ToString()}.jpg");

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