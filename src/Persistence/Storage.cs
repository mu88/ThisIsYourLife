using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessServices;
using DTO.LifePoint;
using Entities;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

// ReSharper disable All - There are so many R# issues due to the usage of EF Core.

// EF Core will handle object initialization
#pragma warning disable 8618

namespace Persistence;

internal class Storage : DbContext, IStorage
{
    private static string _imageDirectory = Path.Combine(UserDirectory, "images");

    private readonly IFileSystem _fileSystem;

    /// <inheritdoc />
    public Storage(DbContextOptions<Storage> options, IFileSystem fileSystem)
        : base(options) =>
        _fileSystem = fileSystem;

    /// <inheritdoc />
    public IQueryable<LifePoint> LifePoints => LifePointsInStorage;

    /// <inheritdoc />
    public IQueryable<Person> Persons => PersonsInStorage;

    public DbSet<LifePoint> LifePointsInStorage { get; set; }

    public DbSet<Person> PersonsInStorage { get; set; }

    internal static string DatabaseDirectory { get; } = Path.Combine(UserDirectory, "db");

    internal static string DatabasePath => Path.Combine(DatabaseDirectory, "ThisIsYourLife.db");

    internal static string UserDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

    /// <inheritdoc />
    public async Task<T?> FindAsync<T>(Guid id)
        where T : class => await Set<T>().FindAsync(id);

    /// <inheritdoc />
    public T? Find<T>(Guid id)
        where T : class =>
        Set<T>().Find(id);

    /// <inheritdoc />
    public async Task<T> AddItemAsync<T>(T itemToAdd)
        where T : class => (await Set<T>().AddAsync(itemToAdd)).Entity;

    /// <inheritdoc />
    public void RemoveItem<T>(T itemToDelete)
        where T : class => Set<T>().Remove(itemToDelete);

    /// <inheritdoc />
    public async Task SaveAsync() => await base.SaveChangesAsync();

    /// <inheritdoc />
    public async Task<Guid> StoreImageAsync(Person owner, ImageToCreate newImage)
    {
        var imageId = Guid.NewGuid();
        var filePathForImage = GetFilePathForImage(owner, imageId);
        await ProcessAndStoreImageAsync(newImage, filePathForImage);

        return imageId;
    }

    /// <inheritdoc />
    public Stream GetImage(Guid ownerId, Guid imageId) => _fileSystem.OpenRead(GetFilePathForImage(ownerId, imageId));

    /// <inheritdoc />
    public void DeleteImage(Guid imageId) => _fileSystem.DeleteFile(Path.Combine(_imageDirectory, imageId.ToString()));

    public async Task EnsureStorageExistsAsync()
    {
        if (!_fileSystem.DirectoryExists(DatabaseDirectory)) { _fileSystem.CreateDirectory(DatabaseDirectory); }

        if (!_fileSystem.FileExists(DatabasePath))
        {
            await Database.EnsureCreatedAsync();
            await Database.MigrateAsync();

            var person = (await AddAsync(new Person("Ultras Dynamo"))).Entity;
            await AddAsync(new LifePoint(new DateOnly(1953, 4, 12),
                                         "Nur die SGD!",
                                         "Wahre Liebe kennt keine Liga",
                                         51.0405849,
                                         13.7478431,
                                         person));
            await SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LifePoint>().ToTable(nameof(LifePoint));
        modelBuilder.Entity<LifePoint>().HasKey(nameof(LifePoint.Id));
        modelBuilder.Entity<LifePoint>().Navigation(point => point.CreatedBy).AutoInclude();
        modelBuilder.Entity<Person>().ToTable(nameof(Person));
        modelBuilder.Entity<Person>().HasKey(nameof(LifePoint.Id));
    }

    private static void ResizeImage(Image image) => image.Mutate(context => context.Resize(new ResizeOptions() { Mode = ResizeMode.Max, Size = new Size(700) }));

    private async Task ProcessAndStoreImageAsync(ImageToCreate newImage, string filePathForImage)
    {
        EnsureImagePathExists(filePathForImage);

        using (var image = await Image.LoadAsync(newImage.Stream))
        {
            ResizeImage(image);
            await image.SaveAsJpegAsync(filePathForImage);
        }
    }

    private void EnsureImagePathExists(string filePathForImage)
    {
        var parentDirectory = Directory.GetParent(filePathForImage) ?? throw new NullReferenceException($"Could not resolve parent directory from {filePathForImage}");
        if (!parentDirectory.Exists) { Directory.CreateDirectory(parentDirectory.ToString()); }
    }

    private string GetFilePathForImage(Person owner, Guid imageId) => GetFilePathForImage(owner.Id, imageId);

    private string GetFilePathForImage(Guid ownerId, Guid imageId) => Path.Combine(_imageDirectory, ownerId.ToString(), $"{imageId.ToString()}.jpg");
}