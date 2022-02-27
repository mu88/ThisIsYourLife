using System;
using System.IO;
using System.Threading.Tasks;
using DTO.LifePoint;
using Entities;

namespace Persistence;

public interface IImageService
{
    Task<Guid> ProcessAndStoreImageAsync(Person newImage, ImageToCreate filePathForImage);

    Stream GetImage(Guid ownerId, Guid imageId);

    void DeleteImage(Guid ownerId, Guid imageId);
}