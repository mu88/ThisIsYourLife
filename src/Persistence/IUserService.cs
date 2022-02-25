using System;

namespace Persistence;

public interface IUserService
{
    public bool UserAlreadySet { get; }

    Guid? Id { get; }

    void SetUser(string name);
}