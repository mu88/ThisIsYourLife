using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
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
        public IQueryable<LifePoint> LifePoints => LifePointsInStorage;

        /// <inheritdoc />
        public IQueryable<Creator> Creators => CreatorsInStorage;

        public DbSet<LifePoint> LifePointsInStorage { get; set; }

        public DbSet<Creator> CreatorsInStorage { get; set; }

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
            modelBuilder.Entity<LifePoint>().ToTable("LifePoint");
            modelBuilder.Entity<Creator>().ToTable("Creator");

            base.OnModelCreating(modelBuilder);
        }
    }
}