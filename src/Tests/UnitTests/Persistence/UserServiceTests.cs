using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Persistence;

namespace Tests.UnitTests.Persistence;

[TestFixture]
public class UserServiceTests
{
    [Test]
    public void SetUser()
    {
        var fileSystemMock = new Mock<IFileSystem>();
        var config = Options.Create(new UserConfig());
        var testee = new UserService(config, fileSystemMock.Object);

        testee.SetUser("Dixie");

        testee.UserAlreadySet.Should().BeTrue();
        testee.Id.Should().NotBeNull();
        fileSystemMock.Verify(system => system.WriteAllText(It.Is<string>(s => s.Contains("user.json")), It.IsAny<string>()));
    }
}