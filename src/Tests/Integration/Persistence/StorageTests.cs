using Entities;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;

namespace Tests.Integration.Persistence;

[TestFixture]
[Category("Integration")]
public class StorageTests
{
    [Test]
    public async Task EnsureStorageExists_InitializesDb()
    {
        var storageOptions = TestStorage.DefaultStorageOptions;
        var dbDir = Path.Combine(storageOptions.BasePath, "db");
        var dbPath = Path.Combine(dbDir, "ThisIsYourLife.db");
        var fileSystemMock = Substitute.For<IFileSystem>();
        fileSystemMock.DirectoryExists(dbDir).Returns(true);
        fileSystemMock.FileExists(dbPath).Returns(false);
        var testee = TestStorage.Create(fileSystemMock);

        await testee.EnsureStorageExistsAsync();

        testee.LifePoints.Should().ContainSingle(point => point.Caption.Equals("Nur die SGD!"));
        testee.Persons.Should().ContainSingle(point => point.Name.Equals("Ultras Dynamo"));
    }

    [Test]
    public async Task FindEntity()
    {
        var person = TestPerson.Create("Dixie");
        var testee = TestStorage.Create();
        await testee.AddItemAsync(person);

        var result = await testee.FindAsync<Person>(person.Id);

        result.Should().Be(person);
    }

    [Test]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0042:Do not use blocking calls in an async method", Justification = "Testing the synchronous Find method")]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1849:Call async methods when in an async method", Justification = "Testing the synchronous Find method")]
    public async Task FindEntitySync()
    {
        var person = TestPerson.Create("Dixie");
        var testee = TestStorage.Create();
        await testee.AddItemAsync(person);

        var result = testee.Find<Person>(person.Id);

        result.Should().Be(person);
    }
}
