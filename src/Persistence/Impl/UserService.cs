using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessServices.Services;
using DTO.Person;
using Microsoft.Extensions.Options;

namespace Persistence;

internal class UserService : IUserService
{
    private readonly IFileSystem _fileSystem;
    private readonly IPersonService _personService;
    private readonly UserConfig _configuration;

    public UserService(IOptions<UserConfig> configuration, IFileSystem fileSystem, IPersonService personService)
    {
        _fileSystem = fileSystem;
        _personService = personService;
        _configuration = configuration.Value;

        if (_configuration.Id != null && !_personService.PersonExists(_configuration.Id.Value))
            throw new ArgumentException($"GUID {_configuration.Id} was provided, but no matching person was found", nameof(configuration));
    }

    public Guid? Id => _configuration.Id;

    /// <inheritdoc />
    public bool UserAlreadySet => Id != null;

    /// <inheritdoc />
    public async Task SetUserAsync(string name)
    {
        await SetNameAndIdAsync(name);
        PersistConfigInFile();
    }

    private void PersistConfigInFile()
    {
        var appSettingsPath = Path.Combine(Storage.UserDirectory, "user.json");
        _fileSystem.WriteAllText(appSettingsPath, JsonSerializer.Serialize(_configuration));
    }

    private async Task SetNameAndIdAsync(string name)
    {
        _configuration.Name = name;
        _configuration.Id = (await _personService.CreatePersonAsync(new PersonToCreate(name))).Id;
    }
}