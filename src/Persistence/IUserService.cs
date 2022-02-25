using System;
using System.Threading.Tasks;

namespace Persistence;

public interface IUserService
{
    public bool UserAlreadySet { get; }

    Guid? Id { get; }

    Task SetUserAsync(string name);
}