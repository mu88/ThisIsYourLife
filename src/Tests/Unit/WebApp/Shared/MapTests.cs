using System.Reflection;
using Bunit;
using BusinessServices.Services;
using DTO.Location;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using NSubstitute;
using NUnit.Framework;
using Persistence;
using Tests.Doubles;
using WebApp;
using WebApp.Shared;
using BunitContext = Bunit.BunitContext;

namespace Tests.Unit.WebApp.Shared;

[TestFixture]
[Category("Unit")]
public class MapTests
{
    [Test]
    public void MapShouldBeInitializedCorrectly()
    {
        var existingLocations = new[] { TestExistingLocation.Create(), TestExistingLocation.Create() };
        var testContext = CreateBunitContext(existingLocations,
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
        var testContext = CreateBunitContext(new[] { TestExistingLocation.Create() },
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

    [Test]
    public async Task SubmittingUserDialog_ShouldRemoveDialogFromMap()
    {
        // Arrange
        var userIsSet = false;
        await using var testContext = CreateBunitContext([],
            configureMapModule: mapModule => mapModule.SetupModule("initializeMap", _ => true),
            configureLifePointDetailModule: module =>
            {
                module.SetupVoid("initialize", _ => true).SetVoidResult();
                module.SetupVoid("enableSpinner").SetVoidResult();
                module.SetupVoid("disableSpinner").SetVoidResult();
            });
        var userService = testContext.Services.GetRequiredService<IUserService>();
        userService.UserAlreadySet.Returns(_ => userIsSet);
        userService.SetUserAsync("Dixie").Returns(_ =>
        {
            userIsSet = true;
            return Task.CompletedTask;
        });
        using var testee = CreateTestee(testContext);
        testee.FindAll("#myModal").Should().ContainSingle();
        await testee.Find("input").ChangeAsync(new ChangeEventArgs { Value = "Dixie" });

        // Act
        await testee.Find("form").SubmitAsync();

        // Assert
        await userService.Received(1).SetUserAsync("Dixie");
        testee.FindAll("#myModal").Should().BeEmpty();
    }

    [Test]
    public async Task OpenPopupForNewLifePointAsync_WhenInteropFails_PropagatesException()
    {
        // Arrange
        await using var testContext = CreateBunitContext([],
            configureMapModule: mapModule => mapModule.SetupModule("initializeMap", _ => true),
            configureLifePointDetailModule: module =>
            {
                module.SetupVoid("initialize", _ => true).SetVoidResult();
                module.SetupVoid("enableSpinner").SetVoidResult();
                module.SetupVoid("disableSpinner").SetVoidResult();
            },
            configureNewLifePointModule: module => module.SetupVoid("createPopupForNewLifePoint", _ => true).SetException(new JSException("Interop failed")));
        using var testee = CreateTestee(testContext);

        // Act
        var act = async () => await testee.Instance.OpenPopupForNewLifePointAsync(1, 2);

        // Assert
        await act.Should().ThrowAsync<JSException>().WithMessage("Interop failed");
    }

    private static IRenderedComponent<Map> CreateTestee(BunitContext testContext) => testContext.Render<Map>();

    private static BunitContext CreateBunitContext(
        IEnumerable<ExistingLocation> existingLocations,
        Action<BunitJSModuleInterop>? configureLifePointDetailModule = null,
        Action<BunitJSModuleInterop>? configureNewLifePointModule = null,
        Action<BunitJSModuleInterop>? configureMapModule = null)
    {
        var testContext = new BunitContext();
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
        testContext.Services.AddSingleton(Options.Create(new MapOptions()));
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

    private static void MarkerForNewLifePointShouldBeAdded(
        double latitude,
        double longitude,
        BunitContext testContext,
        IRenderedComponent<Map> testee)
        => testContext.JSInterop.Invocations.Should()
            .ContainSingle(invocation => invocation.Identifier.Equals("createPopupForNewLifePoint")
                && Equals(GetDotNetObjectReference(testee), invocation.Arguments[0])
                && Equals(GetLeafletMap(testee), invocation.Arguments[1])
                && Equals(latitude, invocation.Arguments[2])
                && Equals(longitude, invocation.Arguments[3]));

    private static void MarkersForExistingLifePointsShouldBeAdded(
        IEnumerable<ExistingLocation> existingLocations,
        BunitContext testContext)
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

    private static void ShouldBeCenteredInDresden(BunitContext testContext, IRenderedComponent<Map> testee)
        => testContext.JSInterop.Invocations.Should()
            .ContainSingle(invocation => invocation.Identifier.Equals("initializeMap")
                && invocation.Arguments[0].As<double>().Equals(51.0405849)
                && invocation.Arguments[1].As<double>().Equals(13.7478431)
                && invocation.Arguments[2].As<int>().Equals(20)
                && invocation.Arguments[3].As<object>().Equals(GetDotNetObjectReference(testee)));
}
