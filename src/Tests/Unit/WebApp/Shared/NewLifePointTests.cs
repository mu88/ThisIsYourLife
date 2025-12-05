using Bunit;
using BusinessServices.Services;
using DTO.LifePoint;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;
using WebApp.Services;
using WebApp.Shared;
using BunitContext = Bunit.BunitContext;

namespace Tests.Unit.WebApp.Shared;

[TestFixture]
[Category("Unit")]
public class NewLifePointTests
{
    [Test]
    public async Task CreateNewLifePoint()
    {
        var lifePointToCreate = TestLifePointToCreate.Create();
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.CreateLifePointAsync(Arg.Any<LifePointToCreate>())
                            .Returns(async _ =>
                            {
                                await Task.Delay(500);
                                return TestExistingLifePoint.From(lifePointToCreate);
                            });
        using var ctx = new BunitContext();
        using var testee = CreateTestee<NewLifePoint>(ctx, lifePointToCreate, lifePointServiceMock);

        EnterInput(testee, lifePointToCreate);
        var task = ClickSaveAsync(testee);

        SpinnerShouldBeDisplayed(testee);
        await task;
        ProposedDateShouldBeCorrect(lifePointToCreate, testee);
        PopupShouldBeRemoved(ctx);
        MarkerShouldBeAdded(ctx);
    }

    [Test]
    public async Task CreateNewLifePointWithImage()
    {
        var imageMemoryStream = new MemoryStream(new byte[10]);
        var browserFileMock = Substitute.For<IBrowserFile>();
        browserFileMock.OpenReadStream(NewLifePoint.MaxAllowedFileSizeInBytes).Returns(imageMemoryStream);
        var lifePointToCreate = TestLifePointToCreate.Create(newImage: TestImageToCreate.Create(imageMemoryStream));
        using var ctx = new BunitContext();
        using var testee = CreateTestee<NewLifePoint>(ctx, lifePointToCreate);

        EnterInput(testee, lifePointToCreate);
        await ClickAndUploadImageAsync(testee, browserFileMock);
        await ClickSaveAsync(testee);

        ProposedDateShouldBeCorrect(lifePointToCreate, testee);
        PopupShouldBeRemoved(ctx);
        MarkerShouldBeAdded(ctx);
        await NewLifePointWithImagePointShouldHaveBeenCreatedAsync(ctx, imageMemoryStream);
    }

    [Test]
    public async Task CreateNewLifePointWithImage_ShouldShowWarning_IfImageIsTooBig()
    {
        var browserFileMock = Substitute.For<IBrowserFile>();
        browserFileMock.OpenReadStream(NewLifePoint.MaxAllowedFileSizeInBytes).Throws<IOException>();
        var lifePointToCreate = TestLifePointToCreate.Create();
        using var ctx = new BunitContext();
        using var testee = CreateTestee<NewLifePoint>(ctx, lifePointToCreate);

        EnterInput(testee, lifePointToCreate);
        await ClickAndUploadImageAsync(testee, browserFileMock);
        await ClickSaveAsync(testee);

        testee.Instance.ImageTooBig.Should().BeTrue();
    }

    [Test]
    public async Task CreateNewLifePointWithImage_ShouldShowWarning_IfUserIsNotSet()
    {
        var userServiceMock = Substitute.For<IUserService>();
        userServiceMock.Id.Returns(null as Guid?);
        var lifePointToCreate = TestLifePointToCreate.Create();
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.CreateLifePointAsync(Arg.Any<LifePointToCreate>())
                            .Returns(async _ =>
                            {
                                await Task.Delay(500);
                                return TestExistingLifePoint.From(lifePointToCreate);
                            });
        using var ctx = new BunitContext();
        using var testee = CreateTestee<NewLifePoint>(ctx, lifePointToCreate, lifePointServiceMock, userServiceMock);

        EnterInput(testee, lifePointToCreate);
        var clickSaveAsync = async () => await ClickSaveAsync(testee);

        await clickSaveAsync.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'User has not been set')");
    }

    [Test]
    public async Task CreateNewLifePointWithImage_ShouldShowWarning_IfInputIsNoImage()
    {
        var imageMemoryStream = new MemoryStream(new byte[10]);
        var browserFileMock = Substitute.For<IBrowserFile>();
        browserFileMock.OpenReadStream(NewLifePoint.MaxAllowedFileSizeInBytes).Returns(imageMemoryStream);
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.CreateLifePointAsync(Arg.Any<LifePointToCreate>()).ThrowsAsync<NoImageException>();
        var lifePointToCreate = TestLifePointToCreate.Create();
        using var ctx = new BunitContext();
        using var testee = CreateTestee<NewLifePoint>(ctx, lifePointToCreate, lifePointServiceMock);

        EnterInput(testee, lifePointToCreate);
        await ClickAndUploadImageAsync(testee, browserFileMock);
        await ClickSaveAsync(testee);

        testee.Instance.InputIsNoImage.Should().BeTrue();
    }

    [Test]
    public async Task OnAfterRenderAsync_ShouldNotUpdatePopup_WhenNewLifePointModuleIsNull()
    {
        var imageMemoryStream = new MemoryStream(new byte[10]);
        var browserFileMock = Substitute.For<IBrowserFile>();
        browserFileMock.OpenReadStream(NewLifePoint.MaxAllowedFileSizeInBytes).Returns(imageMemoryStream);
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.CreateLifePointAsync(Arg.Any<LifePointToCreate>()).ThrowsAsync<NoImageException>();
        var lifePointToCreate = TestLifePointToCreate.Create();
        using var ctx = new BunitContext();
        using var testee = CreateTestee<NewLifePointForTest>(ctx, lifePointToCreate, lifePointServiceMock);

        testee.Instance.ResetNewLifePointModule();
        await testee.Instance.OnAfterRenderForTestAsync(false);

        PopupShouldNotBeUpdated(ctx);
    }

    private static async Task ClickSaveAsync(IRenderedComponent<NewLifePoint> testee) => await testee.Find("button").ClickAsync(new MouseEventArgs());

    private static async Task NewLifePointWithImagePointShouldHaveBeenCreatedAsync(BunitContext testContext, MemoryStream imageMemoryStream)
    {
        var lifePointServiceMock = testContext.Services.GetRequiredService<ILifePointService>();
        await lifePointServiceMock.Received().CreateLifePointAsync(Arg.Is<LifePointToCreate>(create => create.ImageToCreate!.Stream.Equals(imageMemoryStream)));
    }

    private static async Task ClickAndUploadImageAsync(IRenderedComponent<NewLifePoint> testee, IBrowserFile browserFile)
    {
        var filesToUpload = new InputFileChangeEventArgs([browserFile]);
        var inputComponent = testee.FindComponent<InputFile>().Instance;
        await testee.InvokeAsync(() => inputComponent.OnChange.InvokeAsync(filesToUpload));
    }

    private static IRenderedComponent<T> CreateTestee<T>(BunitContext testContext,
                                                         LifePointToCreate lifePointToCreate,
                                                         ILifePointService? lifePointServiceMock = null,
                                                         IUserService? userServiceMock = null)
        where T : NewLifePoint
    {
        var newLifePointDateServiceMock = Substitute.For<INewLifePointDateService>();

        if (userServiceMock == null)
        {
            userServiceMock = Substitute.For<IUserService>();
            userServiceMock.Id.Returns(lifePointToCreate.CreatedBy);
        }

        if (lifePointServiceMock == null)
        {
            lifePointServiceMock = Substitute.For<ILifePointService>();
            lifePointServiceMock.CreateLifePointAsync(Arg.Is<LifePointToCreate>(input => input == lifePointToCreate))
                                .Returns(TestExistingLifePoint.From(lifePointToCreate));
        }

        testContext.Services.AddLocalization();
        testContext.Services.AddSingleton(lifePointServiceMock);
        testContext.Services.AddSingleton(lifePointServiceMock);
        testContext.Services.AddSingleton(Substitute.For<ILogger<NewLifePoint>>());
        testContext.Services.AddSingleton(userServiceMock);
        testContext.Services.AddSingleton(newLifePointDateServiceMock);
#pragma warning disable CS0618
        // Best practice according to: https://stackoverflow.com/questions/72077421/test-event-handler-of-inputfile-in-blazor-component-with-bunit
        testContext.Services.AddSingleton(Options.Create(new RemoteBrowserFileStreamOptions()));
#pragma warning restore CS0618

        var newLifePointModuleInterop = testContext.JSInterop.SetupModule("./Shared/NewLifePoint.razor.js");
        newLifePointModuleInterop.SetupVoid("addMarkerForCreatedLifePoint", Guid.Empty, lifePointToCreate.Latitude, lifePointToCreate.Longitude).SetVoidResult();
        newLifePointModuleInterop.SetupVoid("removePopupForNewLifePoint").SetVoidResult();
        newLifePointModuleInterop.SetupVoid("updatePopup").SetVoidResult();
        testContext.JSInterop.SetupVoid(invocation => string.Equals(invocation.Identifier, "Blazor._internal.InputFile.init", StringComparison.Ordinal)).SetVoidResult();

        var testee = testContext.Render<T>(parameters => parameters
                                                                  .Add(detail => detail.Latitude, lifePointToCreate.Latitude)
                                                                  .Add(detail => detail.Longitude, lifePointToCreate.Longitude));

        return testee;
    }

    private static void MarkerShouldBeAdded(BunitContext testContext) =>
        testContext.JSInterop.Invocations.Should().ContainSingle(invocation => invocation.Identifier.Equals("addMarkerForCreatedLifePoint"));

    private static void PopupShouldBeRemoved(BunitContext testContext) =>
        testContext.JSInterop.Invocations.Should().ContainSingle(invocation => invocation.Identifier.Equals("removePopupForNewLifePoint"));

    private static void PopupShouldNotBeUpdated(BunitContext testContext) =>
        testContext.JSInterop.Invocations.Should().NotContain(invocation => invocation.Identifier.Equals("updatePopup"));

    private static void ProposedDateShouldBeCorrect(LifePointToCreate lifePointToCreate, IRenderedComponent<NewLifePoint> testee) =>
        testee.Services.GetRequiredService<INewLifePointDateService>().ProposedCreationDate.Should().Be(lifePointToCreate.Date);

    private static void EnterInput(IRenderedComponent<NewLifePoint> testee, LifePointToCreate lifePointToCreate)
    {
        testee.Find("[id^=\"input-caption\"]").Change(lifePointToCreate.Caption);
        testee.Find("[id^=\"input-date\"]").Change(lifePointToCreate.Date.ToString("MM-dd-yyyy"));
        testee.Find("[id^=\"input-description\"]").Change(lifePointToCreate.Description);
    }

    private static void SpinnerShouldBeDisplayed(IRenderedComponent<NewLifePoint> testee) => testee.Find("[id^=\"spinner\"]").TextContent.Should().Contain("Saving");

    private class NewLifePointForTest : NewLifePoint
    {
        public void ResetNewLifePointModule() => NewLifePointModule = null!;

        public async Task OnAfterRenderForTestAsync(bool firstRender) => await OnAfterRenderAsync(firstRender);
    }
}