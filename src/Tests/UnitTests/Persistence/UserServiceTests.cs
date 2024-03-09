using BusinessServices.Services;
using DTO.Person;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using Persistence;

namespace Tests.UnitTests.Persistence;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Category("Unit")]
public class UserServiceTests
{
    [Test]
    public async Task SetUser()
    {
        var existingPerson = new ExistingPerson(Guid.NewGuid(), "Dixie");
        var personServiceMock = Substitute.For<IPersonService>();
        personServiceMock.CreatePersonAsync(Arg.Is<PersonToCreate>(create => create.Name == "Dixie")).Returns(existingPerson);
        var fileSystemMock = Substitute.For<IFileSystem>();
        var config = Options.Create(new UserConfig());
        var testee = new UserService(config, Substitute.For<ILogger<UserService>>(), fileSystemMock, personServiceMock);

        await testee.SetUserAsync("Dixie");

        testee.UserAlreadySet.Should().BeTrue();
        testee.Id.Should().NotBeNull();
        fileSystemMock.Received(1).WriteAllText(Arg.Is<string>(s => s.Contains("user.json")),
                                                            Arg.Is<string>(s => s.Contains(existingPerson.Name) && s.Contains(existingPerson.Id.ToString())));
    }

    [Test]
    public void Fail_IfConfigHasNotExistingPerson()
    {
        var id = Guid.NewGuid();
        var personServiceMock = Substitute.For<IPersonService>();
        personServiceMock.PersonExists(id).Returns(false);
        var config = Options.Create(new UserConfig { Id = id });

        var testAction = () => new UserService(config, Substitute.For<ILogger<UserService>>(), Substitute.For<IFileSystem>(), personServiceMock);

        testAction.Should().Throw<ArgumentException>();
    }
}