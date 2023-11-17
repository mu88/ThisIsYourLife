using FluentAssertions;
using NUnit.Framework;
using WebApp.Services;

namespace Tests.UnitTests.WebApp.Services;

[TestFixture]
public class NewLifePointDateServiceTests
{
    [Test]
    public void NowShouldBeProposedDateAsDefault()
    {
        var testee = new NewLifePointDateService();

        var result = testee.ProposedCreationDate;

        result.Should().Be(DateOnly.FromDateTime(DateTime.Now));
    }

    [Test]
    public void PreviouslySetDateShouldBeProposalForNextTime()
    {
        var proposal = new DateOnly(1953, 4, 12);
        var testee = new NewLifePointDateService { ProposedCreationDate = proposal };

        var result = testee.ProposedCreationDate;

        result.Should().Be(proposal);
    }
}