using BusinessServices;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Persistence;

namespace Tests.Doubles;

public static class TestStorage
{
    public static IStorage Create(IFileSystem? fileSystem = null, IImageService? imageService = null)
    {
        var storage = new Storage(new DbContextOptionsBuilder<Storage>().UseSqlite(CreateInMemoryDatabase()).Options,
            Substitute.For<ILogger<Storage>>(),
            fileSystem ?? Substitute.For<IFileSystem>(),
            imageService ?? Substitute.For<IImageService>());
        storage.Database.EnsureCreated();

        return storage;
    }

    private static SqliteConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        return connection;
    }
}
