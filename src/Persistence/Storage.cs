using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BusinessServices;
using Entities;
using Microsoft.EntityFrameworkCore;

// ReSharper disable All - There are so many R# issues due to the usage of EF Core.

// EF Core will handle object initialization
#pragma warning disable 8618

namespace Persistence
{
    // TODO mu88: Make this internal as soon as seeding of test data is not longer necessary
    public class Storage : DbContext, IStorage
    {
        private readonly IFileSystem _fileSystem;

        /// <inheritdoc />
        public Storage(DbContextOptions<Storage> options, IFileSystem fileSystem)
            : base(options)
        {
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public IQueryable<LifePoint> LifePoints => LifePointsInStorage;

        /// <inheritdoc />
        public IQueryable<Person> Persons => PersonsInStorage;

        public DbSet<LifePoint> LifePointsInStorage { get; set; }

        public DbSet<Person> PersonsInStorage { get; set; }

        /// <inheritdoc />
        public async Task<T?> FindAsync<T>(Guid id)
            where T : class => await Set<T>().FindAsync(id);

        /// <inheritdoc />
        public async Task<T> AddItemAsync<T>(T itemToAdd)
            where T : class => (await Set<T>().AddAsync(itemToAdd)).Entity;

        /// <inheritdoc />
        public void RemoveItem<T>(T itemToDelete)
            where T : class => Set<T>().Remove(itemToDelete);

        /// <inheritdoc />
        public async Task SaveAsync() => await base.SaveChangesAsync();

        /// <inheritdoc />
        public async Task<Guid> StoreImageAsync(Stream imageStream)
        {
            var imageId = Guid.NewGuid();
            var filePathForImage = GetFilePathForImage(imageId);
            await _fileSystem.CreateFileAsync(filePathForImage, imageStream);

            return imageId;
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LifePoint>().ToTable("LifePoint");
            modelBuilder.Entity<LifePoint>().HasKey(nameof(LifePoint.Id));
            modelBuilder.Entity<LifePoint>().Navigation(point => point.CreatedBy).AutoInclude();
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Person>().HasKey(nameof(LifePoint.Id));
        }

        private static string GetFilePathForImage(Guid imageId)
        {
            // TODO mu88: Rethink file path retrieval - should this be configurable?
            var entryLocation = Assembly.GetEntryAssembly()!.Location;
            return Path.Combine(entryLocation, "images", imageId.ToString());
        }
    }
}