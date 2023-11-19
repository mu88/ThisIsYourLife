using BusinessServices;
using DTO.LifePoint;
using Entities;
using Logging.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// ReSharper disable All - There are so many R# issues due to the usage of EF Core.

// EF Core will handle object initialization
#pragma warning disable 8618

namespace Persistence;

internal class Storage : DbContext, IStorage
{
    private readonly ILogger<Storage> _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IImageService _imageService;

    /// <inheritdoc />
    public Storage(DbContextOptions<Storage> options, ILogger<Storage> logger, IFileSystem fileSystem, IImageService imageService)
        : base(options)
    {
        _logger = logger;
        _fileSystem = fileSystem;
        _imageService = imageService;
    }

    /// <inheritdoc />
    public IQueryable<LifePoint> LifePoints => LifePointsInStorage;

    /// <inheritdoc />
    public IQueryable<Person> Persons => PersonsInStorage;

    public DbSet<LifePoint> LifePointsInStorage { get; set; }

    public DbSet<Person> PersonsInStorage { get; set; }

    internal static string DatabaseDirectory { get; } = Path.Combine(UserDirectory, "db");

    internal static string DatabasePath => Path.Combine(DatabaseDirectory, "ThisIsYourLife.db");

    internal static string UserDirectory => Path.Combine("/home", "app", "data");

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
    public async Task<Guid> StoreImageAsync(Person owner, ImageToCreate newImage) => await _imageService.ProcessAndStoreImageAsync(owner, newImage);

    /// <inheritdoc />
    public Stream GetImage(Guid ownerId, Guid imageId) => _imageService.GetImage(ownerId, imageId);

    /// <inheritdoc />
    public void DeleteImage(Guid ownerId, Guid imageId) => _imageService.DeleteImage(ownerId, imageId);

    public async Task EnsureStorageExistsAsync()
    {
        _logger.MethodStarted();

        if (!_fileSystem.DirectoryExists(DatabaseDirectory))
        {
            _logger.CreatingDatabaseDirectory(DatabaseDirectory);
            _fileSystem.CreateDirectory(DatabaseDirectory);
        }

        if (!_fileSystem.FileExists(DatabasePath))
        {
            await CreateDatabase();
            await SeedData();
        }

        _logger.MethodFinished();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _logger.ExecutingBaseOnModelCreating();
        base.OnModelCreating(modelBuilder);

        _logger.ExecutingCustomOnModelCreating();
        modelBuilder.Entity<LifePoint>().ToTable(nameof(LifePoint));
        modelBuilder.Entity<LifePoint>().HasKey(nameof(LifePoint.Id));
        modelBuilder.Entity<LifePoint>().Navigation(point => point.CreatedBy).AutoInclude();
        modelBuilder.Entity<Person>().ToTable(nameof(Person));
        modelBuilder.Entity<Person>().HasKey(nameof(LifePoint.Id));
    }

    private async Task SeedData()
    {
        _logger.SeedingData();

        var person = (await AddAsync(new Person("Ultras Dynamo"))).Entity;
        await AddAsync(new LifePoint(new DateOnly(1953, 4, 12),
                                     "Nur die SGD!",
                                     "Wahre Liebe kennt keine Liga",
                                     51.0405849,
                                     13.7478431,
                                     person));
        await SaveChangesAsync();
    }

    private async Task CreateDatabase()
    {
        _logger.CreatingDatabase(DatabasePath);

        await Database.EnsureCreatedAsync();
        await Database.MigrateAsync();

        _logger.DatabaseCreated(DatabasePath);
    }
}