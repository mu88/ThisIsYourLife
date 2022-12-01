using System.Threading.Tasks;
using Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;

namespace Tests.IntegrationTests.Persistence;

[TestFixture]
public class StorageTests
{
    [Test]
    public async Task EnsureStorageExists_InitializesDb()
    {
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(system => system.DirectoryExists(Storage.DatabaseDirectory)).Returns(true);
        fileSystemMock.Setup(system => system.FileExists(Storage.DatabasePath)).Returns(false);
        var testee = TestStorage.Create(fileSystemMock.Object);

        await testee.EnsureStorageExistsAsync();

        testee.LifePoints.Should().ContainSingle(point => point.Caption.Equals("Nur die SGD!"));
        testee.Persons.Should().ContainSingle(point => point.Name.Equals("Ultras Dynamo"));
    }
    
    [Test]
    public async Task FindEntity()
    {
        Person person = TestPerson.Create("Dixie");
        var testee = TestStorage.Create();
        await testee.AddItemAsync(person);

        var result = testee.Find<Person>(person.Id);

        result.Should().Be(person);
    }
}