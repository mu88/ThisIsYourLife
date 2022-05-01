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
using Tests.Doubles;
using WebApp.Shared;
using TestContext = Bunit.TestContext;

namespace Tests.UnitTests.WebApp.Shared;

[TestFixture]
public class LifePointDetailTests
{
    [Test]
    public void CreatedLifePointDetail_ShouldBeRenderedProperly()
    {
        var existingLifePoint = TestExistingLifePoint.Create();
        var id = existingLifePoint.Id;
        var lifePointServiceMock = new Mock<ILifePointService>();
        lifePointServiceMock.Setup(service => service.GetLifePointAsync(id)).ReturnsAsync(existingLifePoint);

        using var testee = CreateTestee(lifePointServiceMock, id);

        ShouldBeRenderedProperly(testee, existingLifePoint);
    }

    [Test]
    public void DeleteExistingLifePoint()
    {
        var existingLifePoint = TestExistingLifePoint.Create();
        var id = existingLifePoint.Id;
        var lifePointServiceMock = new Mock<ILifePointService>();
        lifePointServiceMock.Setup(service => service.GetLifePointAsync(id)).ReturnsAsync(existingLifePoint);
        var lifePointDetailModuleMock = new Mock<IJSObjectReference>();
        using var testee = CreateTestee(lifePointServiceMock, id, lifePointDetailModuleMock);

        ClickDelete(testee);

        LifePointShouldBeDeleted(lifePointServiceMock, id);
        MarkerShouldBeRemoved(lifePointDetailModuleMock, id);
    }

    private static void ClickDelete(IRenderedComponent<LifePointDetail> testee) => testee.Find("button").Click();

    private static void MarkerShouldBeRemoved(Mock<IJSObjectReference> lifePointDetailModuleMock, Guid id) =>
        lifePointDetailModuleMock.Verify(lifePointDetailModule =>
                                             lifePointDetailModule.InvokeAsync<IJSVoidResult>("removeMarkerOfLifePoint", new object?[] { id.ToString() }),
                                         Times.Once);

    private static void LifePointShouldBeDeleted(Mock<ILifePointService> lifePointServiceMock, Guid id) =>
        lifePointServiceMock.Verify(service => service.DeleteLifePointAsync(id), Times.Once);

    private static void ShouldBeRenderedProperly(IRenderedFragment testee, ExistingLifePoint existingLifePoint)
    {
        testee.Find("img").Attributes["src"].Should().NotBeNull();
        testee.Find("img").Attributes["src"]!.Value.Should().Be($"http://localhost/api/images/{existingLifePoint.CreatedBy.Id}/{existingLifePoint.ImageId}");

        testee.Find("h5").TextContent.Should().Be(existingLifePoint.Caption);

        testee.Find("h6").TextContent.Should().Match($"On *05*2022 with {existingLifePoint.CreatedBy.Name}");

        testee.Find("p").TextContent.Should().Be(existingLifePoint.Description);
    }

    private static IRenderedComponent<LifePointDetail> CreateTestee(IMock<ILifePointService> lifePointServiceMock,
                                                                    Guid id,
                                                                    Mock<IJSObjectReference>? lifePointDetailModuleMock = null)
    {
        lifePointDetailModuleMock ??= new Mock<IJSObjectReference>();
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock
            .Setup(runtime => runtime.InvokeAsync<IJSObjectReference>("import", new object?[] { "./Shared/LifePointDetail.razor.js" }))
            .ReturnsAsync(lifePointDetailModuleMock.Object);
        var ctx = new TestContext();
        ctx.Services.AddLocalization();
        ctx.Services.AddSingleton(lifePointServiceMock.Object);
        ctx.Services.AddSingleton(jsRuntimeMock.Object);
        ctx.Services.AddSingleton(new Mock<ILogger<LifePointDetail>>().Object);
        var testee = ctx.RenderComponent<LifePointDetail>(parameters => parameters.Add(detail => detail.Id, id.ToString()));

        return testee;
    }
}