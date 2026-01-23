using AngleSharp.Html.Dom;
using Bunit;
using BusinessServices.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tests.Doubles;
using WebApp.Shared;
using BunitContext = Bunit.BunitContext;

#pragma warning disable CA1861

namespace Tests.Unit.WebApp.Shared;

[TestFixture]
[Category("Unit")]
public class FilterLifePointsTests
{
    [Test]
    public void EnableFiltering_ShouldLoadCreators()
    {
        using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);

        ClickFilterButton(testee);

        DistinctCreatorsShouldBeDisplayed(testee);
    }

    [Test]
    public void EnableFiltering_ShouldLoadYears()
    {
        using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);

        ClickFilterButton(testee);

        DistinctYearsShouldBeDisplayed(testee);
    }

    [Test]
    public async Task FilterByYear_ShouldFilterMarkers()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByYearAsync(testee, 1953);

        MarkersShouldBeDisplayed(testContext);
    }

    [Test]
    public async Task FilterByYear_ShouldShowAllMarkers_WhenChoosingDefaultYear()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByYearAsync(testee, 1953);
        await ResetYearFilterToDefaultAsync(testee);

        MarkersShouldBeDisplayed(testContext, 2);
    }

    [TestCase("tralala")]
    [TestCase(null)]
    public async Task FilterByYear_ShouldNotFail_WhenInputIsNoYear(object? changedValue)
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        var testAction = async () => await ChangeYearSelectElementAsync(testee, changedValue);

        await testAction.Should().NotThrowAsync();
    }

    [Test]
    public async Task FilterByCreator_ShouldFilterMarkers()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByCreatorAsync(testee, testContext, "Dixie");

        MarkersShouldBeDisplayed(testContext);
    }

    [Test]
    public async Task FilterByCreator_ShouldShowAllMarkers_WhenChoosingDefaultCreator()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByCreatorAsync(testee, testContext, "Dixie");
        await ResetCreatorFilterToDefaultAsync(testee);

        MarkersShouldBeDisplayed(testContext, 2);
    }

    [Test]
    public async Task FilterByCreator_ShouldDisableYearFiltering()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByCreatorAsync(testee, testContext, "Dixie");

        YearFilteringShouldBeDisabled(testee);
    }

    [TestCase(1953)]
    [TestCase(null)]
    public async Task FilterByCreator_ShouldNotFail_WhenInputIsNoCreator(object? changedValue)
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        var testAction = async () => await ChangeCreatorSelectElementAsync(testee, changedValue);

        await testAction.Should().NotThrowAsync();
    }

    [Test]
    public async Task FilterByCreator_ShouldDoNothing_WhenGuidsMatch()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        var testAction = async () => await ChangeCreatorSelectElementAsync(testee, Guid.Empty);

        await testAction.Should().NotThrowAsync();
    }

    [Test]
    public async Task FilterByYear_ShouldNotReloadMarkers_WhenSameYearHasBeenChosen()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);
        await FilterByYearAsync(testee, 1953);

        await FilterByYearAsync(testee, 1953);

        MarkersShouldBeDisplayed(testContext);
    }

    [Test]
    public async Task FilterByYear_ShouldDisableCreatorFiltering()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByYearAsync(testee, 1953);

        CreatorFilteringShouldBeDisabled(testee);
    }

    [Test]
    public async Task FilterByCreator_ShouldNotReloadMarkers_WhenSameCreatorHasBeenChosen()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);
        await FilterByCreatorAsync(testee, testContext, "Dixie");

        await FilterByCreatorAsync(testee, testContext, "Dixie");

        MarkersShouldBeDisplayed(testContext);
    }

    [Test]
    public async Task DisableFiltering_ShouldReloadAllMarkers()
    {
        await using var testContext = new BunitContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee); // enables filtering
        await FilterByYearAsync(testee, 1953);

        ClickFilterButton(testee); // disables filtering

        MarkersShouldBeDisplayed(testContext, 2);
    }

    private static IRenderedComponent<FilterLifePoints> CreateTestee(BunitContext testContext)
    {
        var existingPerson1 = TestExistingPerson.Create("Dixie");
        var existingPerson2 = TestExistingPerson.Create("Ulf");
        var existingLocation = TestExistingLocation.Create();

        var lifePointServiceMock = Substitute.For<ILifePointService>();
        lifePointServiceMock.GetDistinctCreators().Returns([existingPerson1, existingPerson2]);
        lifePointServiceMock.GetDistinctCreators(1953).Returns([existingPerson1]);
        lifePointServiceMock.GetDistinctCreators(1954).Returns([existingPerson2]);
        lifePointServiceMock.GetDistinctYears().Returns([1953, 1954]);
        lifePointServiceMock.GetDistinctYears(existingPerson1.Id).Returns([1953]);
        lifePointServiceMock.GetDistinctYears(existingPerson2.Id).Returns([1954]);
        lifePointServiceMock.GetAllLocations().Returns([existingLocation]);
        lifePointServiceMock.GetAllLocations(1953).Returns([existingLocation]);
        lifePointServiceMock.GetAllLocations(null, existingPerson1.Id).Returns([existingLocation]);
        lifePointServiceMock.GetAllLocations(1953, existingPerson1.Id).Returns([existingLocation]);

        testContext.Services.AddSingleton(lifePointServiceMock);
        testContext.Services.AddLocalization();
        var lifePointDetailModule = testContext.JSInterop.SetupModule("./Shared/LifePointDetail.razor.js");
        lifePointDetailModule.SetupVoid("reset").SetVoidResult();
        lifePointDetailModule.SetupVoid("enableSpinner").SetVoidResult();
        lifePointDetailModule.SetupVoid("disableSpinner").SetVoidResult();
        lifePointDetailModule.SetupVoid("createMarkerForExistingLifePoint", existingLocation.Id, existingLocation.Latitude, existingLocation.Longitude)
                             .SetVoidResult();

        var testee = testContext.Render<FilterLifePoints>();

        return testee;
    }

    private static async Task ChangeYearSelectElementAsync(IRenderedComponent<FilterLifePoints> testee, object? value) =>
        await testee.Find("[id^=\"distinctYear\"]").ChangeAsync(new ChangeEventArgs { Value = value });

    private static async Task ChangeCreatorSelectElementAsync(IRenderedComponent<FilterLifePoints> testee, object? value) =>
        await testee.Find("[id^=\"distinctCreator\"]").ChangeAsync(new ChangeEventArgs { Value = value });

    private static void DistinctCreatorsShouldBeDisplayed(IRenderedComponent<FilterLifePoints> testee)
    {
        var creatorSelectElement = testee.Find("[id^=\"distinctCreator\"]");
        creatorSelectElement.Children.Should().HaveCount(3).And.Subject.Should().AllSatisfy(element => element.Should().BeAssignableTo<IHtmlOptionElement>());
        creatorSelectElement.Children[0].As<IHtmlOptionElement>().Label.Should().Be("Created by");
        creatorSelectElement.Children[1].As<IHtmlOptionElement>().Label.Should().Be("Dixie");
        creatorSelectElement.Children[2].As<IHtmlOptionElement>().Label.Should().Be("Ulf");
    }

    private static void DistinctYearsShouldBeDisplayed(IRenderedComponent<FilterLifePoints> testee)
    {
        var yearSelectElement = testee.Find("[id^=\"distinctYear\"]");
        yearSelectElement.Children.Should().HaveCount(3).And.Subject.Should().AllSatisfy(element => element.Should().BeAssignableTo<IHtmlOptionElement>());
        yearSelectElement.Children[0].As<IHtmlOptionElement>().Label.Should().Be("Created in");
        yearSelectElement.Children[1].As<IHtmlOptionElement>().Label.Should().Be("1953");
        yearSelectElement.Children[2].As<IHtmlOptionElement>().Label.Should().Be("1954");
    }

    private static void CreatorFilteringShouldBeDisabled(IRenderedComponent<FilterLifePoints> testee) =>
        testee.Find("[id^=\"distinctCreator\"]").HasAttribute("disabled").Should().BeTrue();

    private static void YearFilteringShouldBeDisabled(IRenderedComponent<FilterLifePoints> testee) =>
        testee.Find("[id^=\"distinctYear\"]").HasAttribute("disabled").Should().BeTrue();

    private static void MarkersShouldBeDisplayed(BunitContext testContext, int numberOfCalls = 1)
    {
        testContext.JSInterop.VerifyInvoke("reset", numberOfCalls);

        var (latitude, longitude, id) = testContext.Services.GetRequiredService<ILifePointService>().GetAllLocations().First();
        testContext.JSInterop.VerifyInvoke("createMarkerForExistingLifePoint", numberOfCalls)
                   .Should()
                   .AllSatisfy(invocation => invocation.Arguments.Should().BeEquivalentTo(new object[] { id, latitude, longitude }));
    }

    private static async Task FilterByYearAsync(IRenderedComponent<FilterLifePoints> testee, int year) => await ChangeYearSelectElementAsync(testee, year);

    private static async Task ResetYearFilterToDefaultAsync(IRenderedComponent<FilterLifePoints> testee) => await ChangeYearSelectElementAsync(testee, -1);

    private static async Task FilterByCreatorAsync(IRenderedComponent<FilterLifePoints> testee, BunitContext testContext, string name)
    {
        var idForName = testContext.Services.GetRequiredService<ILifePointService>()
                                   .GetDistinctCreators()
                                   .Single(person => person.Name.Equals(name))
                                   .Id;

        await ChangeCreatorSelectElementAsync(testee, idForName);
    }

    private static async Task ResetCreatorFilterToDefaultAsync(IRenderedComponent<FilterLifePoints> testee) => await ChangeCreatorSelectElementAsync(testee, Guid.Empty);

    private static void ClickFilterButton(IRenderedComponent<FilterLifePoints> testee) => testee.Find("[id^=\"filterButton\"]").Click();
}