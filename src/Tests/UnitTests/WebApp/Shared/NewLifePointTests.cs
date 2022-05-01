using System;
using Bunit;
using BusinessServices.Services;
using DTO.LifePoint;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
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
    public void CreateNewLifePoint()
    {
        var lifePointToCreate = TestLifePointToCreate.Create();
        using var testee = CreateTestee(lifePointToCreate);

        EnterInput(testee, lifePointToCreate);
        ClickSave(testee);

        ProposedDateShouldBeCorrect(lifePointToCreate, testee);
        PopupShouldBeRemoved(testee);
        MarkerShouldBeAdded(lifePointToCreate, testee);
    }

    private static void ClickSave(IRenderedComponent<NewLifePoint> testee) => testee.Find("button").Click();

    private static IRenderedComponent<NewLifePoint> CreateTestee(LifePointToCreate lifePointToCreate)
    {
        var newLifePointDateServiceMock = new Mock<INewLifePointDateService>();
        newLifePointDateServiceMock.SetupProperty(service => service.ProposedCreationDate);

        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(service => service.Id).Returns(lifePointToCreate.CreatedBy);

        var lifePointServiceMock = new Mock<ILifePointService>();
        lifePointServiceMock
            .Setup(service => service.CreateLifePointAsync(It.Is<LifePointToCreate>(input => input == lifePointToCreate)))
            .ReturnsAsync(TestExistingLifePoint.From(lifePointToCreate));

        var newLifePointModuleMock = new Mock<IJSObjectReference>();
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock
            .Setup(runtime => runtime.InvokeAsync<IJSObjectReference>("import", new object?[] { "./Shared/NewLifePoint.razor.js" }))
            .ReturnsAsync(newLifePointModuleMock.Object);

        var ctx = new TestContext();
        ctx.Services.AddLocalization();
        ctx.Services.AddSingleton(lifePointServiceMock.Object);
        ctx.Services.AddSingleton(jsRuntimeMock.Object);
        ctx.Services.AddSingleton(new Mock<ILogger<NewLifePoint>>().Object);
        ctx.Services.AddSingleton(userServiceMock.Object);
        ctx.Services.AddSingleton(newLifePointDateServiceMock.Object);
        ctx.Services.AddSingleton(newLifePointModuleMock);

        var testee = ctx.RenderComponent<NewLifePoint>(parameters => parameters
                                                           .Add(detail => detail.Latitude, lifePointToCreate.Latitude)
                                                           .Add(detail => detail.Longitude, lifePointToCreate.Longitude));

        return testee;
    }

    private void MarkerShouldBeAdded(LifePointToCreate lifePointToCreate, IRenderedComponent<NewLifePoint> testee)
    {
        var newLifePointModuleMock = testee.Services.GetRequiredService<Mock<IJSObjectReference>>();
        newLifePointModuleMock.Verify(reference => reference.InvokeAsync<IJSVoidResult>("addMarkerForCreatedLifePoint",
                                                                                        new object?[]
                                                                                        {
                                                                                            Guid.Empty, lifePointToCreate.Latitude, lifePointToCreate.Longitude
                                                                                        }),
                                      Times.Once);
    }

    private void PopupShouldBeRemoved(IRenderedComponent<NewLifePoint> testee)
    {
        var newLifePointModuleMock = testee.Services.GetRequiredService<Mock<IJSObjectReference>>();
        newLifePointModuleMock.Verify(reference => reference.InvokeAsync<IJSVoidResult>("removePopupForNewLifePoint", new object?[] { }), Times.Once);
    }

    private void ProposedDateShouldBeCorrect(LifePointToCreate lifePointToCreate, IRenderedComponent<NewLifePoint> testee) =>
        testee.Services.GetRequiredService<INewLifePointDateService>().ProposedCreationDate.Should().Be(lifePointToCreate.Date);

    private void EnterInput(IRenderedComponent<NewLifePoint> testee, LifePointToCreate lifePointToCreate)
    {
        testee.Find("[id^=\"input-caption\"]").Change(lifePointToCreate.Caption);
        testee.Find("[id^=\"input-date\"]").Change(lifePointToCreate.Date.ToString("MM-dd-yyyy"));
        testee.Find("[id^=\"input-description\"]").Change(lifePointToCreate.Description);
    }
}