using System;

namespace WebApp.Services;

public interface INewLifePointDateService
{
    public DateOnly ProposedCreationDate { get; set; }
}