using System.Reflection;
using Bunit;
using BusinessServices.Services;
using DTO.Location;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;
using WebApp.Shared;
using TestContext = Bunit.TestContext;

namespace Tests.UnitTests.WebApp.Shared;

[TestFixture]
public class MapTests
{
    [Test]
    public void MapShouldBeInitializedCorrectly()
    {
        var existingLocations = new[] { TestExistingLocation.Create(), TestExistingLocation.Create() };
        var leafletMapMock = new Mock<IJSObjectReference>();
        var lifePointDetailModuleMock = new Mock<IJSObjectReference>();

        using var testee = CreateTestee(existingLocations, lifePointDetailModuleMock: lifePointDetailModuleMock, leafletMapMock: leafletMapMock);

        ShouldBeCenteredInDresden(leafletMapMock, testee);
        MarkersForExistingLifePointsShouldBeAdded(existingLocations, lifePointDetailModuleMock);
    }

    [Test]
    public async Task OpenPopupForNewLifePointAsync()
    {
        const double latitude = 19;
        const double longitude = 19;
        var newLifePointModuleMock = new Mock<IJSObjectReference>();
        using var testee = CreateTestee(new[] { TestExistingLocation.Create() }, newLifePointModuleMock);

        await testee.Instance.OpenPopupForNewLifePointAsync(latitude, longitude);

        MarkerForNewLifePointShouldBeAdded(latitude, longitude, newLifePointModuleMock, testee);
    }

    private static IRenderedComponent<Map> CreateTestee(IEnumerable<ExistingLocation> existingLocations,
                                                        Mock<IJSObjectReference>? newLifePointModuleMock = null,
                                                        Mock<IJSObjectReference>? lifePointDetailModuleMock = null,
                                                        Mock<IJSObjectReference>? leafletMapMock = null)
    {
        var lifePointServiceMock = new Mock<ILifePointService>();
        lifePointServiceMock.Setup(service => service.GetAllLocations(null, null)).Returns(existingLocations);

        newLifePointModuleMock ??= new Mock<IJSObjectReference>();
        lifePointDetailModuleMock ??= new Mock<IJSObjectReference>();
        leafletMapMock ??= new Mock<IJSObjectReference>();
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock
            .Setup(runtime => runtime.InvokeAsync<IJSObjectReference>("import", new object?[] { "./Shared/NewLifePoint.razor.js" }))
            .ReturnsAsync(newLifePointModuleMock.Object);
        jsRuntimeMock
            .Setup(runtime => runtime.InvokeAsync<IJSObjectReference>("import", new object?[] { "./Shared/LifePointDetail.razor.js" }))
            .ReturnsAsync(lifePointDetailModuleMock.Object);
        jsRuntimeMock
            .Setup(runtime => runtime.InvokeAsync<IJSObjectReference>("import", new object?[] { "./Shared/Map.razor.js" }))
            .ReturnsAsync(leafletMapMock.Object);

        var ctx = new TestContext();
        ctx.Services.AddLocalization();
        ctx.Services.AddSingleton(jsRuntimeMock.Object);
        ctx.Services.AddSingleton(new Mock<ILogger<Map>>().Object);
        ctx.Services.AddSingleton(new Mock<IUserService>().Object);
        ctx.Services.AddSingleton(new Mock<IPersonService>().Object);
        ctx.Services.AddSingleton(lifePointServiceMock.Object);

        return ctx.RenderComponent<Map>();
    }

    private static object? GetLeafletMap(IRenderedComponent<Map> testee) => GetPrivateFieldFromTestee(testee, "_leafletMap");

    private static object? GetPrivateFieldFromTestee(IRenderedComponent<Map> testee, string fieldName)
    {
        // Using reflection is a really bad idea, but using It.IsAny<> does not work
        var fieldInfo = testee.Instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        var leafletMap = fieldInfo!.GetValue(testee.Instance);
        return leafletMap;
    }

    private static object? GetDotNetObjectReference(IRenderedComponent<Map> testee) => GetPrivateFieldFromTestee(testee, "_objRef");

    private static void MarkerForNewLifePointShouldBeAdded(double latitude,
                                                    double longitude,
                                                    Mock<IJSObjectReference> newLifePointModuleMock,
                                                    IRenderedComponent<Map> testee)
    {
        newLifePointModuleMock.Verify(reference => reference.InvokeAsync<IJSVoidResult>("createPopupForNewLifePoint",
                                                                                        new[]
                                                                                        {
                                                                                            GetDotNetObjectReference(testee),
                                                                                            GetLeafletMap(testee),
                                                                                            latitude,
                                                                                            longitude
                                                                                        }));
    }

    private static void MarkersForExistingLifePointsShouldBeAdded(IEnumerable<ExistingLocation> existingLocations,
                                                           Mock<IJSObjectReference> lifePointDetailModuleMock)
    {
        foreach (var existingLocation in existingLocations)
        {
            lifePointDetailModuleMock.Verify(reference => reference.InvokeAsync<IJSVoidResult>("createMarkerForExistingLifePoint",
                                                                                               new object[]
                                                                                               {
                                                                                                   existingLocation.Id,
                                                                                                   existingLocation.Latitude,
                                                                                                   existingLocation.Longitude
                                                                                               }));
        }
    }

    private static void ShouldBeCenteredInDresden(Mock<IJSObjectReference> leafletMapMock, IRenderedComponent<Map> testee)
    {
        leafletMapMock.Verify(reference => reference.InvokeAsync<IJSObjectReference>("initializeMap",
                                                                                     new[] { 51.0405849, 13.7478431, 20, GetDotNetObjectReference(testee) }));
    }
}