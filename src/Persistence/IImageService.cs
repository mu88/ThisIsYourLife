using DTO.LifePoint;
using Entities;

namespace Persistence;

public interface IImageService
{
    Task<Guid> ProcessAndStoreImageAsync(Person owner, ImageToCreate newImage);

    Stream GetImage(Guid ownerId, Guid imageId);

    void DeleteImage(Guid ownerId, Guid imageId);
}
