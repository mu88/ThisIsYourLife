using AngleSharp.Html.Dom;
using Bunit;
using BusinessServices.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Tests.Doubles;
using WebApp.Shared;
using TestContext = Bunit.TestContext;

namespace Tests.UnitTests.WebApp.Shared;

[TestFixture]
public class FilterLifePointsTests
{
    [Test]
    public void EnableFiltering_ShouldLoadCreators()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);

        ClickFilterButton(testee);

        DistinctCreatorsShouldBeDisplayed(testee);
    }

    [Test]
    public void EnableFiltering_ShouldLoadYears()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);

        ClickFilterButton(testee);

        DistinctYearsShouldBeDisplayed(testee);
    }

    [Test]
    public async Task FilterByYear_ShouldFilterMarkers()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByYearAsync(testee, 1953);

        MarkersShouldBeDisplayed(testContext);
    }

    [Test]
    public async Task FilterByYear_ShouldShowAllMarkers_WhenChoosingDefaultYear()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByYearAsync(testee, 1953);
        await ResetYearFilterToDefaultAsync(testee);

        MarkersShouldBeDisplayed(testContext, 2);
    }

    [Test]
    public async Task FilterByYear_ShouldNotFail_WhenInputIsNoYear()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        var testAction = async () => await ChangeYearSelectElementAsync(testee, "tralala");

        await testAction.Should().NotThrowAsync();
    }

    [Test]
    public async Task FilterByCreator_ShouldFilterMarkers()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByCreatorAsync(testee, testContext, "Dixie");

        MarkersShouldBeDisplayed(testContext);
    }

    [Test]
    public async Task FilterByCreator_ShouldShowAllMarkers_WhenChoosingDefaultCreator()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByCreatorAsync(testee, testContext, "Dixie");
        await ResetCreatorFilterToDefaultAsync(testee);

        MarkersShouldBeDisplayed(testContext, 2);
    }

    [Test]
    public async Task FilterByCreator_ShouldDisableYearFiltering()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByCreatorAsync(testee, testContext, "Dixie");

        YearFilteringShouldBeDisabled(testee);
    }

    [Test]
    public async Task FilterByCreator_ShouldNotFail_WhenInputIsNoCreator()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        var testAction = async () => await ChangeCreatorSelectElementAsync(testee, 1953);

        await testAction.Should().NotThrowAsync();
    }

    [Test]
    public async Task FilterByYear_ShouldNotReloadMarkers_WhenSameYearHasBeenChosen()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);
        await FilterByYearAsync(testee, 1953);

        await FilterByYearAsync(testee, 1953);

        MarkersShouldBeDisplayed(testContext);
    }

    [Test]
    public async Task FilterByYear_ShouldDisableCreatorFiltering()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);

        await FilterByYearAsync(testee, 1953);

        CreatorFilteringShouldBeDisabled(testee);
    }

    [Test]
    public async Task FilterByCreator_ShouldNotReloadMarkers_WhenSameCreatorHasBeenChosen()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee);
        await FilterByCreatorAsync(testee, testContext, "Dixie");

        await FilterByCreatorAsync(testee, testContext, "Dixie");

        MarkersShouldBeDisplayed(testContext);
    }

    [Test]
    public async Task DisableFiltering_ShouldReloadAllMarkers()
    {
        using var testContext = new TestContext();
        using var testee = CreateTestee(testContext);
        ClickFilterButton(testee); // enables filtering
        await FilterByYearAsync(testee, 1953);

        ClickFilterButton(testee); // disables filtering

        MarkersShouldBeDisplayed(testContext, 2);
    }

    private static IRenderedComponent<FilterLifePoints> CreateTestee(TestContext testContext)
    {
        var existingPerson1 = TestExistingPerson.Create("Dixie");
        var existingPerson2 = TestExistingPerson.Create("Ulf");
        var existingLocation = TestExistingLocation.Create();

        var lifePointServiceMock = new Mock<ILifePointService>();
        lifePointServiceMock.Setup(service => service.GetDistinctCreators(null)).Returns(new[] { existingPerson1, existingPerson2 });
        lifePointServiceMock.Setup(service => service.GetDistinctCreators(1953)).Returns(new[] { existingPerson1 });
        lifePointServiceMock.Setup(service => service.GetDistinctCreators(1954)).Returns(new[] { existingPerson2 });
        lifePointServiceMock.Setup(service => service.GetDistinctYears(null)).Returns(new[] { 1953, 1954 });
        lifePointServiceMock.Setup(service => service.GetDistinctYears(existingPerson1.Id)).Returns(new[] { 1953 });
        lifePointServiceMock.Setup(service => service.GetDistinctYears(existingPerson2.Id)).Returns(new[] { 1954 });
        lifePointServiceMock.Setup(service => service.GetAllLocations(null, null)).Returns(new[] { existingLocation });
        lifePointServiceMock.Setup(service => service.GetAllLocations(1953, null)).Returns(new[] { existingLocation });
        lifePointServiceMock.Setup(service => service.GetAllLocations(null, existingPerson1.Id)).Returns(new[] { existingLocation });
        lifePointServiceMock.Setup(service => service.GetAllLocations(1953, existingPerson1.Id)).Returns(new[] { existingLocation });

        testContext.Services.AddSingleton(lifePointServiceMock.Object);
        testContext.Services.AddLocalization();
        var lifePointDetailModule = testContext.JSInterop.SetupModule("./Shared/LifePointDetail.razor.js");
        lifePointDetailModule.SetupVoid("reset").SetVoidResult();
        lifePointDetailModule.SetupVoid("enableSpinner").SetVoidResult();
        lifePointDetailModule.SetupVoid("disableSpinner").SetVoidResult();
        lifePointDetailModule.SetupVoid("createMarkerForExistingLifePoint", existingLocation.Id, existingLocation.Latitude, existingLocation.Longitude)
            .SetVoidResult();

        var testee = testContext.RenderComponent<FilterLifePoints>();

        return testee;
    }

    private static async Task ChangeYearSelectElementAsync(IRenderedComponent<FilterLifePoints> testee, object value) =>
        await testee.Find("[id^=\"distinctYear\"]").ChangeAsync(new ChangeEventArgs { Value = value });

    private static async Task ChangeCreatorSelectElementAsync(IRenderedComponent<FilterLifePoints> testee, object value) =>
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

    private static void MarkersShouldBeDisplayed(TestContext testContext, int numberOfCalls = 1)
    {
        testContext.JSInterop.VerifyInvoke("reset", numberOfCalls);

        var (latitude, longitude, id) = testContext.Services.GetRequiredService<ILifePointService>().GetAllLocations().First();
        testContext.JSInterop.VerifyInvoke("createMarkerForExistingLifePoint", numberOfCalls)
            .Should()
            .AllSatisfy(invocation => invocation.Arguments.Should().BeEquivalentTo(new object[] { id, latitude, longitude }));
    }

    private static async Task FilterByYearAsync(IRenderedComponent<FilterLifePoints> testee, int year) => await ChangeYearSelectElementAsync(testee, year);

    private static async Task ResetYearFilterToDefaultAsync(IRenderedComponent<FilterLifePoints> testee) => await ChangeYearSelectElementAsync(testee, -1);

    private static async Task FilterByCreatorAsync(IRenderedComponent<FilterLifePoints> testee, TestContext testContext, string name)
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