using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Persistence;
using WebApp.Shared;
using BunitContext = Bunit.BunitContext;

namespace Tests.Unit.WebApp.Shared;

[TestFixture]
[Category("Unit")]
public class UserDialogTests
{
    [Test]
    public async Task ClickingOk_ShouldSetUsername()
    {
        var userServiceMock = Substitute.For<IUserService>();
        using var testee = CreateTestee(userServiceMock);

        EnterUsername(testee, "Dixie");
        ClickSubmit(testee);

        await userServiceMock.Received(1).SetUserAsync("Dixie");
    }

    [Test]
    public async Task ClickingOk_ShouldNotSetUsername_IfNothingHasBeenEntered()
    {
        var userServiceMock = Substitute.For<IUserService>();
        using var testee = CreateTestee(userServiceMock);

        ClickSubmit(testee);

        await userServiceMock.DidNotReceive().SetUserAsync(Arg.Any<string>());
    }

    private static void ClickSubmit(IRenderedComponent<UserDialog> testee) => testee.Find("button").Click();

    private static void EnterUsername(IRenderedComponent<UserDialog> testee, string username) => testee.Find("input").Change(username);

    private static IRenderedComponent<UserDialog> CreateTestee(IUserService userServiceMock)
    {
        var ctx = new BunitContext();
        ctx.Services.AddLocalization();
        ctx.Services.AddSingleton(userServiceMock);
        var testee = ctx.Render<UserDialog>();
        return testee;
    }
}
