using System;
using System.Linq;
using System.Threading.Tasks;
using Entities;

namespace BusinessServices
{
    public interface IStorage
    {
        public IQueryable<LifePoint> LifePoints { get; }

        public IQueryable<Person> Persons { get; }

        public Task<T> GetAsync<T>(Guid id)
            where T : class;

        public Task<T> AddItemAsync<T>(T itemToAdd)
            where T : class;

        public void RemoveItem<T>(T itemToDelete)
            where T : class;

        public Task SaveAsync();
    }
}