namespace WebApp.Services;

internal class NewLifePointDateService(TimeProvider timeProvider) : INewLifePointDateService
{
    /// <inheritdoc />
    public DateOnly ProposedCreationDate { get; set; } = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
}
