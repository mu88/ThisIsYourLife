using System;
using System.Threading.Tasks;
using BusinessServices.Services;
using DTO.Person;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Persistence;

namespace Tests.UnitTests.Persistence;

[TestFixture]
public class UserServiceTests
{
    [Test]
    public async Task SetUser()
    {
        var existingPerson = new ExistingPerson(Guid.NewGuid(), "Dixie");
        var personServiceMock = new Mock<IPersonService>();
        personServiceMock.Setup(service => service.CreatePersonAsync(It.Is<PersonToCreate>(create => create.Name == "Dixie"))).ReturnsAsync(existingPerson);
        var fileSystemMock = new Mock<IFileSystem>();
        var config = Options.Create(new UserConfig());
        var testee = new UserService(config, new Mock<ILogger<UserService>>().Object, fileSystemMock.Object, personServiceMock.Object);

        await testee.SetUserAsync("Dixie");

        testee.UserAlreadySet.Should().BeTrue();
        testee.Id.Should().NotBeNull();
        fileSystemMock.Verify(system => system.WriteAllText(It.Is<string>(s => s.Contains("user.json")),
                                                            It.Is<string>(s => s.Contains(existingPerson.Name) && s.Contains(existingPerson.Id.ToString()))));
    }

    [Test]
    public void Fail_IfConfigHasNotExistingPerson()
    {
        var id = Guid.NewGuid();
        var personServiceMock = new Mock<IPersonService>();
        personServiceMock.Setup(service => service.PersonExists(id)).Returns(false);
        var config = Options.Create(new UserConfig { Id = id });

        var testAction = () => new UserService(config, new Mock<ILogger<UserService>>().Object, new Mock<IFileSystem>().Object, personServiceMock.Object);

        testAction.Should().Throw<ArgumentException>();
    }
}