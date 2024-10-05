using System.Reflection;
using Bunit;
using BusinessServices.Services;
using DTO.LifePoint;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Tests.Doubles;
using WebApp.Shared;
using TestContext = Bunit.TestContext;

namespace Tests.UnitTests.WebApp.Shared;

[TestFixture]
[Category("Unit")]
public class LifePointDetailTests
{
    [Test]
    public void CreatedLifePointDetail_ShouldBeRenderedProperly()
    {
        var existingLifePoint = TestExistingLifePoint.Create();
        var id = existingLifePoint.Id;
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.GetLifePointAsync(id).Returns(existingLifePoint);
        var testContext = CreateTestContext(lifePointServiceMock);

        using var testee = CreateTestee(testContext, id);

        ShouldBeRenderedProperly(testee, existingLifePoint);
    }

    [Test]
    public async Task UpdatePopup()
    {
        var existingLifePoint = TestExistingLifePoint.Create();
        var id = existingLifePoint.Id;
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.GetLifePointAsync(id).Returns(existingLifePoint);
        var testContext = CreateTestContext(lifePointServiceMock, module => module.SetupVoid("updatePopup", _ => true).SetVoidResult());
        using var testee = CreateTestee(testContext, id);

        await (Task)typeof(LifePointDetail)
                    .GetTypeInfo()
                    .GetMethod("UpdatePopupAsync", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(testee.Instance, Array.Empty<object>())!;

        PopupShouldBeUpdated(testContext, testee.Instance.Id);
    }

    [Test]
    public async Task OnAfterRenderAsync_ShouldUpdatePopup()
    {
        var existingLifePoint = TestExistingLifePoint.From(TestLifePointToCreate.Create());
        var id = existingLifePoint.Id;
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.GetLifePointAsync(id).Returns(existingLifePoint);
        var testContext = CreateTestContext(lifePointServiceMock, module => module.SetupVoid("updatePopup", _ => true).SetVoidResult());
        using var testee = testContext.RenderComponent<LifePointDetailForTest>(parameters => parameters.Add(detail => detail.Id, id.ToString()));

        await testee.Instance.OnAfterRenderForTestAsync(false);

        PopupShouldBeUpdated(testContext, testee.Instance.Id);
    }

    [Test]
    public async Task OnAfterRenderAsync_ShouldNotUpdatePopup_WhenLifePointDetailModuleIsNull()
    {
        var existingLifePoint = TestExistingLifePoint.From(TestLifePointToCreate.Create());
        var id = existingLifePoint.Id;
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.GetLifePointAsync(id).Returns(existingLifePoint);
        var testContext = CreateTestContext(lifePointServiceMock, module => module.SetupVoid("updatePopup", _ => true).SetVoidResult());
        using var testee = testContext.RenderComponent<LifePointDetailForTest>(parameters => parameters.Add(detail => detail.Id, id.ToString()));

        testee.Instance.ResetLifePointDetailModule();
        await testee.Instance.OnAfterRenderForTestAsync(false);

        PopupShouldNotBeUpdated(testContext, testee.Instance.Id);
    }

    [Test]
    public async Task DeleteExistingLifePoint()
    {
        var existingLifePoint = TestExistingLifePoint.Create();
        var id = existingLifePoint.Id;
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.GetLifePointAsync(id).Returns(existingLifePoint);
        var testContext = CreateTestContext(lifePointServiceMock);
        using var testee = CreateTestee(testContext, id);

        ClickDelete(testee);

        await LifePointShouldBeDeletedAsync(lifePointServiceMock, id);
        MarkerShouldBeRemoved(testContext, id);
    }

    private static void ClickDelete(IRenderedComponent<LifePointDetail> testee) => testee.Find("button").Click();

    private static void MarkerShouldBeRemoved(TestContext testContext, Guid id) =>
        testContext.JSInterop.Invocations.Should()
                   .ContainSingle(invocation => invocation.Identifier.Equals("removeMarkerOfLifePoint")
                                                && Equals(id.ToString(), invocation.Arguments[0]));

    private static void PopupShouldBeUpdated(TestContext testContext, string id) =>
        testContext.JSInterop.Invocations.Should()
                   .ContainSingle(invocation => invocation.Identifier.Equals("updatePopup")
                                                && Equals(id, invocation.Arguments[0]));

    private static void PopupShouldNotBeUpdated(TestContext testContext, string id) =>
        testContext.JSInterop.Invocations.Should()
                   .NotContain(invocation => invocation.Identifier.Equals("updatePopup")
                                             && Equals(id, invocation.Arguments[0]));

    private static async Task LifePointShouldBeDeletedAsync(ILifePointService lifePointServiceMock, Guid id) =>
        await lifePointServiceMock.Received(1).DeleteLifePointAsync(id);

    private static void ShouldBeRenderedProperly(IRenderedFragment testee, ExistingLifePoint existingLifePoint)
    {
        testee.Find("img").Attributes["src"].Should().NotBeNull();
        testee.Find("img").Attributes["src"]!.Value.Should().Be($"http://localhost/api/images/{existingLifePoint.CreatedBy.Id}/{existingLifePoint.ImageId}");

        testee.Find("h5").TextContent.Should().Be(existingLifePoint.Caption);

        testee.Find("h6").TextContent.Should().Match($"On *{DateTime.Now.Month}*{DateTime.Now.Year} with {existingLifePoint.CreatedBy.Name}");

        testee.Find("p").TextContent.Should().Be(existingLifePoint.Description);
    }

    private static IRenderedComponent<LifePointDetail> CreateTestee(TestContext testContext, Guid id) =>
        testContext.RenderComponent<LifePointDetail>(parameters => parameters.Add(detail => detail.Id, id.ToString()));

    private static TestContext CreateTestContext(ILifePointService lifePointServiceMock, Action<BunitJSModuleInterop>? configureLifePointDetailModule = null)
    {
        var testContext = new TestContext();
        var lifePointDetailModule = testContext.JSInterop.SetupModule("./Shared/LifePointDetail.razor.js");
        configureLifePointDetailModule?.Invoke(lifePointDetailModule);
        testContext.Services.AddLocalization();
        testContext.Services.AddSingleton(lifePointServiceMock);
        testContext.Services.AddSingleton(Substitute.For<ILogger<LifePointDetail>>());
        var configurationMock = Substitute.For<IConfiguration>();
        configurationMock["CanDelete"].Returns("true");
        testContext.Services.AddSingleton(configurationMock);

        return testContext;
    }

    private class LifePointDetailForTest : LifePointDetail
    {
        public void ResetLifePointDetailModule() => LifePointDetailModule = null!;

        public async Task OnAfterRenderForTestAsync(bool firstRender) => await base.OnAfterRenderAsync(firstRender);
    }
}