using System.Data.Common;
using BusinessServices;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Tests.Doubles
{
    public static class TestStorage
    {
        public static IStorage Create()
        {
            var storage = new Storage(new DbContextOptionsBuilder<Storage>().UseSqlite(CreateInMemoryDatabase()).Options);
            storage.Database.EnsureCreated();

            return storage;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            return connection;
        }
    }
}