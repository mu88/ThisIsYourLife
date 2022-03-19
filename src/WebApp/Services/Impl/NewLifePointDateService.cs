using System;

namespace WebApp.Services;

public class NewLifePointDateService : INewLifePointDateService
{
    public NewLifePointDateService() => ProposedCreationDate = DateOnly.FromDateTime(DateTime.Now);

    /// <inheritdoc />
    public DateOnly ProposedCreationDate { get; set; }
}