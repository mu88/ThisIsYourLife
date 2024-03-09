using System.Reflection;
using Bunit;
using BusinessServices.Services;
using DTO.Location;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;
using WebApp.Shared;
using TestContext = Bunit.TestContext;

namespace Tests.UnitTests.WebApp.Shared;

[TestFixture]
[Category("Unit")]
public class MapTests
{
    [Test]
    public void MapShouldBeInitializedCorrectly()
    {
        var existingLocations = new[] { TestExistingLocation.Create(), TestExistingLocation.Create() };
        var testContext = CreateTestContext(existingLocations,
                                            configureMapModule: mapModule => mapModule.SetupModule("initializeMap", _ => true),
                                            configureLifePointDetailModule: lifePointDetailModule =>
                                            {
                                                lifePointDetailModule.SetupVoid("initialize", _ => true).SetVoidResult();
                                                lifePointDetailModule.SetupVoid("enableSpinner").SetVoidResult();
                                                lifePointDetailModule.SetupVoid("disableSpinner").SetVoidResult();
                                                lifePointDetailModule.SetupVoid("createMarkerForExistingLifePoint", _ => true).SetVoidResult();
                                            });

        using var testee = CreateTestee(testContext);

        ShouldBeCenteredInDresden(testContext, testee);
        MarkersForExistingLifePointsShouldBeAdded(existingLocations, testContext);
    }

    [Test]
    public async Task OpenPopupForNewLifePointAsync()
    {
        const double latitude = 19;
        const double longitude = 19;
        var testContext = CreateTestContext(new []{TestExistingLocation.Create()},
                                            lifePointDetailModule =>
                                            {
                                                lifePointDetailModule.SetupVoid("initialize", _ => true).SetVoidResult();
                                                lifePointDetailModule.SetupVoid("enableSpinner").SetVoidResult();
                                                lifePointDetailModule.SetupVoid("disableSpinner").SetVoidResult();
                                                lifePointDetailModule.SetupVoid("createMarkerForExistingLifePoint", _ => true).SetVoidResult();
                                            },
                                            newLifePointModule => newLifePointModule.SetupVoid("createPopupForNewLifePoint", _ => true).SetVoidResult(),
                                            mapModule => mapModule.SetupModule("initializeMap", _ => true));
        using var testee = CreateTestee(testContext);

        await testee.Instance.OpenPopupForNewLifePointAsync(latitude, longitude);

        MarkerForNewLifePointShouldBeAdded(latitude, longitude, testContext, testee);
    }

    private static IRenderedComponent<Map> CreateTestee(TestContext testContext) => testContext.RenderComponent<Map>();

    private static TestContext CreateTestContext(IEnumerable<ExistingLocation> existingLocations,
                                                 Action<BunitJSModuleInterop>? configureLifePointDetailModule = null,
                                                 Action<BunitJSModuleInterop>? configureNewLifePointModule = null,
                                                 Action<BunitJSModuleInterop>? configureMapModule = null)
    {
        var testContext = new TestContext();
        var lifePointDetailModule = testContext.JSInterop.SetupModule("./Shared/LifePointDetail.razor.js");
        configureLifePointDetailModule?.Invoke(lifePointDetailModule);
        var mapModule = testContext.JSInterop.SetupModule("./Shared/Map.razor.js");
        configureMapModule?.Invoke(mapModule);
        var newLifePointModule = testContext.JSInterop.SetupModule("./Shared/NewLifePoint.razor.js");
        configureNewLifePointModule?.Invoke(newLifePointModule);
        testContext.Services.AddLocalization();
        testContext.Services.AddSingleton(Substitute.For<ILogger<Map>>());
        testContext.Services.AddSingleton(Substitute.For<IUserService>());
        testContext.Services.AddSingleton(Substitute.For<IPersonService>());
        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.GetAllLocations().Returns(existingLocations);
        testContext.Services.AddSingleton(lifePointServiceMock);

        return testContext;
    }

    private static object? GetLeafletMap(IRenderedComponent<Map> testee) => GetPrivateFieldFromTestee(testee, "_leafletMap");

    private static object? GetPrivateFieldFromTestee(IRenderedComponent<Map> testee, string fieldName)
    {
        // Using reflection is a really bad idea, but using Arg.Any<> does not work
        var fieldInfo = testee.Instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        var leafletMap = fieldInfo!.GetValue(testee.Instance);
        return leafletMap;
    }

    private static object? GetDotNetObjectReference(IRenderedComponent<Map> testee) => GetPrivateFieldFromTestee(testee, "_objRef");

    private static void MarkerForNewLifePointShouldBeAdded(double latitude,
                                                           double longitude,
                                                           TestContext testContext,
                                                           IRenderedComponent<Map> testee) =>
        testContext.JSInterop.Invocations.Should()
            .ContainSingle(invocation => invocation.Identifier.Equals("createPopupForNewLifePoint")
                                         && Equals(GetDotNetObjectReference(testee), invocation.Arguments[0])
                                         && Equals(GetLeafletMap(testee), invocation.Arguments[1])
                                         && Equals(latitude, invocation.Arguments[2])
                                         && Equals(longitude, invocation.Arguments[3]));

    private static void MarkersForExistingLifePointsShouldBeAdded(IEnumerable<ExistingLocation> existingLocations,
                                                                  TestContext testContext)
    {
        foreach (var existingLocation in existingLocations)
        {
            testContext.JSInterop.Invocations.Should()
                .ContainSingle(invocation => invocation.Identifier.Equals("createMarkerForExistingLifePoint")
                                             && invocation.Arguments[0].As<Guid>().Equals(existingLocation.Id)
                                             && invocation.Arguments[1].As<double>().Equals(existingLocation.Latitude)
                                             && invocation.Arguments[2].As<double>().Equals(existingLocation.Longitude));
        }
    }

    private static void ShouldBeCenteredInDresden(TestContext testContext, IRenderedComponent<Map> testee) =>
        testContext.JSInterop.Invocations.Should()
            .ContainSingle(invocation => invocation.Identifier.Equals("initializeMap")
                                         && invocation.Arguments[0].As<double>().Equals(51.0405849)
                                         && invocation.Arguments[1].As<double>().Equals(13.7478431)
                                         && invocation.Arguments[2].As<int>().Equals(20)
                                         && invocation.Arguments[3].As<object>().Equals(GetDotNetObjectReference(testee)));
}