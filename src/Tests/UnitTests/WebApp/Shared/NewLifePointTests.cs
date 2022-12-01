using System;
using System.IO;
using System.Threading.Tasks;
using Bunit;
using BusinessServices.Services;
using DTO.LifePoint;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;
using WebApp.Services;
using WebApp.Shared;
using TestContext = Bunit.TestContext;

namespace Tests.UnitTests.WebApp.Shared;

[TestFixture]
public class NewLifePointTests
{
    [Test]
    public async Task CreateNewLifePoint()
    {
        var lifePointToCreate = TestLifePointToCreate.Create();
        var lifePointServiceMock = new Mock<ILifePointService>();
        lifePointServiceMock
            .Setup(service => service.CreateLifePointAsync(It.IsAny<LifePointToCreate>()))
            .ReturnsAsync(TestExistingLifePoint.From(lifePointToCreate), TimeSpan.FromSeconds(1));
        using var ctx = new TestContext();
        using var testee = CreateTestee(ctx, lifePointToCreate, lifePointServiceMock);

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
        var browserFileMock = new Mock<IBrowserFile>();
        browserFileMock.Setup(file => file.OpenReadStream(NewLifePoint.MaxAllowedFileSizeInBytes, default)).Returns(imageMemoryStream);
        var lifePointToCreate = TestLifePointToCreate.Create(newImage: TestImageToCreate.Create(imageMemoryStream));
        using var ctx = new TestContext();
        using var testee = CreateTestee(ctx, lifePointToCreate);

        EnterInput(testee, lifePointToCreate);
        await ClickAndUploadImageAsync(testee, browserFileMock);
        await ClickSaveAsync(testee);

        ProposedDateShouldBeCorrect(lifePointToCreate, testee);
        PopupShouldBeRemoved(ctx);
        MarkerShouldBeAdded(ctx);
        NewLifeWithImagePointShouldHaveBeenCreated(ctx, imageMemoryStream);
    }

    [Test]
    public async Task CreateNewLifeWithImage_ShouldShowWarning_IfImageIsTooBig()
    {
        var browserFileMock = new Mock<IBrowserFile>();
        browserFileMock.Setup(file => file.OpenReadStream(NewLifePoint.MaxAllowedFileSizeInBytes, default)).Throws<IOException>();
        var lifePointToCreate = TestLifePointToCreate.Create();
        using var ctx = new TestContext();
        using var testee = CreateTestee(ctx, lifePointToCreate);

        EnterInput(testee, lifePointToCreate);
        await ClickAndUploadImageAsync(testee, browserFileMock);
        await ClickSaveAsync(testee);

        testee.Instance.ImageTooBig.Should().BeTrue();
    }

    [Test]
    public async Task CreateNewLifeWithImage_ShouldShowWarning_IfInputIsNoImage()
    {
        var imageMemoryStream = new MemoryStream(new byte[10]);
        var browserFileMock = new Mock<IBrowserFile>();
        browserFileMock.Setup(file => file.OpenReadStream(NewLifePoint.MaxAllowedFileSizeInBytes, default)).Returns(imageMemoryStream);
        var lifePointServiceMock = new Mock<ILifePointService>();
        lifePointServiceMock.Setup(service => service.CreateLifePointAsync(It.IsAny<LifePointToCreate>())).Throws<NoImageException>();
        var lifePointToCreate = TestLifePointToCreate.Create();
        using var ctx = new TestContext();
        using var testee = CreateTestee(ctx, lifePointToCreate, lifePointServiceMock);

        EnterInput(testee, lifePointToCreate);
        await ClickAndUploadImageAsync(testee, browserFileMock);
        await ClickSaveAsync(testee);

        testee.Instance.InputIsNoImage.Should().BeTrue();
    }

    private static async Task ClickSaveAsync(IRenderedComponent<NewLifePoint> testee) => await testee.Find("button").ClickAsync(new MouseEventArgs());

    private static void NewLifeWithImagePointShouldHaveBeenCreated(TestContextBase testContext, MemoryStream imageMemoryStream)
    {
        var lifePointServiceMock = testContext.Services.GetRequiredService<Mock<ILifePointService>>();
        lifePointServiceMock.Verify(service => service.CreateLifePointAsync(It.Is<LifePointToCreate>(create => create.ImageToCreate!.Stream.Equals(imageMemoryStream))));
    }

    private static async Task ClickAndUploadImageAsync(IRenderedComponent<NewLifePoint> testee, IMock<IBrowserFile> browserFile)
    {
        var filesToUpload = new InputFileChangeEventArgs(new[] { browserFile.Object });
        var inputComponent = testee.FindComponent<InputFile>().Instance;
        await testee.InvokeAsync(() => inputComponent.OnChange.InvokeAsync(filesToUpload));
    }

    private static IRenderedComponent<NewLifePoint> CreateTestee(TestContext testContext,
                                                          LifePointToCreate lifePointToCreate,
                                                          Mock<ILifePointService>? lifePointServiceMock = null)
    {
        var newLifePointDateServiceMock = new Mock<INewLifePointDateService>();
        newLifePointDateServiceMock.SetupProperty(service => service.ProposedCreationDate);

        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(service => service.Id).Returns(lifePointToCreate.CreatedBy);

        if (lifePointServiceMock == null)
        {
            lifePointServiceMock = new Mock<ILifePointService>();
            lifePointServiceMock
                .Setup(service => service.CreateLifePointAsync(It.Is<LifePointToCreate>(input => input == lifePointToCreate)))
                .ReturnsAsync(TestExistingLifePoint.From(lifePointToCreate));
        }

        testContext.Services.AddLocalization();
        testContext.Services.AddSingleton(lifePointServiceMock.Object);
        testContext.Services.AddSingleton(lifePointServiceMock);
        testContext.Services.AddSingleton(new Mock<ILogger<NewLifePoint>>().Object);
        testContext.Services.AddSingleton(userServiceMock.Object);
        testContext.Services.AddSingleton(newLifePointDateServiceMock.Object);
#pragma warning disable CS0618
        // Best practice according to: https://stackoverflow.com/questions/72077421/test-event-handler-of-inputfile-in-blazor-component-with-bunit
        testContext.Services.AddSingleton(Options.Create(new RemoteBrowserFileStreamOptions()));
#pragma warning restore CS0618

        var newLifePointModuleInterop = testContext.JSInterop.SetupModule("./Shared/NewLifePoint.razor.js");
        newLifePointModuleInterop.SetupVoid("addMarkerForCreatedLifePoint", Guid.Empty, lifePointToCreate.Latitude, lifePointToCreate.Longitude).SetVoidResult();
        newLifePointModuleInterop.SetupVoid("removePopupForNewLifePoint").SetVoidResult();
        newLifePointModuleInterop.SetupVoid("updatePopup").SetVoidResult();
        testContext.JSInterop.SetupVoid(invocation => invocation.Identifier == "Blazor._internal.InputFile.init").SetVoidResult();

        var testee = testContext.RenderComponent<NewLifePoint>(parameters => parameters
                                                                   .Add(detail => detail.Latitude, lifePointToCreate.Latitude)
                                                                   .Add(detail => detail.Longitude, lifePointToCreate.Longitude));

        return testee;
    }

    private static void MarkerShouldBeAdded(TestContext testContext) =>
        testContext.JSInterop.Invocations.Should().ContainSingle(invocation => invocation.Identifier.Equals("addMarkerForCreatedLifePoint"));

    private static void PopupShouldBeRemoved(TestContext testContext) =>
        testContext.JSInterop.Invocations.Should().ContainSingle(invocation => invocation.Identifier.Equals("removePopupForNewLifePoint"));

    private static void ProposedDateShouldBeCorrect(LifePointToCreate lifePointToCreate, IRenderedComponent<NewLifePoint> testee) =>
        testee.Services.GetRequiredService<INewLifePointDateService>().ProposedCreationDate.Should().Be(lifePointToCreate.Date);

    private static void EnterInput(IRenderedComponent<NewLifePoint> testee, LifePointToCreate lifePointToCreate)
    {
        testee.Find("[id^=\"input-caption\"]").Change(lifePointToCreate.Caption);
        testee.Find("[id^=\"input-date\"]").Change(lifePointToCreate.Date.ToString("MM-dd-yyyy"));
        testee.Find("[id^=\"input-description\"]").Change(lifePointToCreate.Description);
    }

    private static void SpinnerShouldBeDisplayed(IRenderedComponent<NewLifePoint> testee) => testee.Find("[id^=\"spinner\"]").TextContent.Should().Contain("Saving");
}