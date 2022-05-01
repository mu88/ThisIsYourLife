using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Persistence;
using WebApp.Shared;
using TestContext = Bunit.TestContext;

namespace Tests.UnitTests.WebApp.Shared;

[TestFixture]
public class UserDialogTests
{
    [Test]
    public void ClickingOk_ShouldSetUsername()
    {
        var userServiceMock = new Mock<IUserService>();
        using var testee = CreateTestee(userServiceMock);

        EnterUsername(testee, "Dixie");
        ClickSubmit(testee);

        userServiceMock.Verify(service => service.SetUserAsync("Dixie"), Times.Once);
    }

    [Test]
    public void ClickingOk_ShouldNotSetUsername_IfNothingHasBeenEntered()
    {
        var userServiceMock = new Mock<IUserService>();
        using var testee = CreateTestee(userServiceMock);

        ClickSubmit(testee);

        userServiceMock.Verify(service => service.SetUserAsync(It.IsAny<string>()), Times.Never);
    }

    private static void ClickSubmit(IRenderedComponent<UserDialog> testee) => testee.Find("button").Click();

    private static void EnterUsername(IRenderedComponent<UserDialog> testee, string username) => testee.Find("input").Change(username);

    private static IRenderedComponent<UserDialog> CreateTestee(IMock<IUserService> userServiceMock)
    {
        var ctx = new TestContext();
        ctx.Services.AddLocalization();
        ctx.Services.AddSingleton(userServiceMock.Object);
        var testee = ctx.RenderComponent<UserDialog>();
        return testee;
    }
}