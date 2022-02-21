using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DTO.LifePoint;
using Entities;

namespace BusinessServices;

public interface IStorage
{
    public IQueryable<LifePoint> LifePoints { get; }

    public IQueryable<Person> Persons { get; }

    public Task<T?> FindAsync<T>(Guid id)
        where T : class;

    public Task<T> AddItemAsync<T>(T itemToAdd)
        where T : class;

    public void RemoveItem<T>(T itemToDelete)
        where T : class;

    public Task SaveAsync();

    public Task<Guid> StoreImageAsync(Person owner, ImageToCreate newImage);

    Stream GetImage(Guid imageId);

    void DeleteImage(Guid imageId);
}