namespace WebApp.Services;

internal class NewLifePointDateService : INewLifePointDateService
{
    /// <inheritdoc />
    public DateOnly ProposedCreationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}
