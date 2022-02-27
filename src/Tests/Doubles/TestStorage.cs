using System.Data.Common;
using BusinessServices;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;

namespace Tests.Doubles;

public static class TestStorage
{
    public static IStorage Create(IFileSystem? fileSystem = null, IImageService? imageService = null)
    {
        var storage = new Storage(new DbContextOptionsBuilder<Storage>().UseSqlite(CreateInMemoryDatabase()).Options,
                                  fileSystem ?? new Mock<IFileSystem>().Object,
                                  imageService ?? new Mock<IImageService>().Object);
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