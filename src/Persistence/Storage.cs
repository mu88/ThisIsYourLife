using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessServices;
using Entities;
using Microsoft.EntityFrameworkCore;

// ReSharper disable All - There are so many R# issues due to the usage of EF Core.

// EF Core will handle object initialization
#pragma warning disable 8618

namespace Persistence
{
    public class Storage : DbContext, IStorage
    {
        /// <inheritdoc />
        public Storage(DbContextOptions<Storage> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        public IQueryable<LifePoint> LifePoints => LifePointsInStorage;

        /// <inheritdoc />
        public IQueryable<Person> Persons => PersonsInStorage;

        public DbSet<LifePoint> LifePointsInStorage { get; set; }

        public DbSet<Person> PersonsInStorage { get; set; }

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(Guid id)
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LifePoint>().ToTable("LifePoint");
            modelBuilder.Entity<LifePoint>().HasKey(nameof(LifePoint.Id));
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Person>().HasKey(nameof(LifePoint.Id));
        }
    }
}