using System;

namespace WebApp.Services;

internal class NewLifePointDateService : INewLifePointDateService
{
    public NewLifePointDateService() => ProposedCreationDate = DateOnly.FromDateTime(DateTime.Now);

    /// <inheritdoc />
    public DateOnly ProposedCreationDate { get; set; }
}