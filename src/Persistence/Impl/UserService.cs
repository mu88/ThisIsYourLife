using System.Text.Json;
using BusinessServices.Services;
using DTO.Person;
using Logging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Persistence;

internal class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IPersonService _personService;
    private readonly UserConfig _configuration;

    public UserService(IOptions<UserConfig> configuration, ILogger<UserService> logger, IFileSystem fileSystem, IPersonService personService)
    {
        _logger = logger;
        _fileSystem = fileSystem;
        _personService = personService;
        _configuration = configuration.Value;

        if (_configuration.Id != null && !_personService.PersonExists(_configuration.Id.Value))
        {
            throw new ArgumentException($"GUID {_configuration.Id} was provided, but no matching person was found", nameof(configuration));
        }
    }

    public Guid? Id => _configuration.Id;

    /// <inheritdoc />
    public bool UserAlreadySet => Id != null;

    /// <inheritdoc />
    public async Task SetUserAsync(string name) =>
        await _logger.LogMethodStartAndEndAsync(async () =>
        {
            await SetNameAndIdAsync(name);
            PersistConfigInFile();
            _logger.UserSet(name);
        });

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