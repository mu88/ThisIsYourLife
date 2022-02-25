using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace BusinessServices.Services;

internal class UserService : IUserService
{
    private readonly UserConfig _configuration;

    public UserService(IOptions<UserConfig> configuration) => _configuration = configuration.Value;

    public Guid? Id => _configuration.Id;

    /// <inheritdoc />
    public bool UserAlreadySet => Id != null;

    /// <inheritdoc />
    public void SetUser(string name)
    {
        SetNameAndId(name);
        PersistConfigInFile();
    }

    private void PersistConfigInFile()
    {
        var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user.json");
        File.WriteAllText(appSettingsPath, JsonSerializer.Serialize(_configuration));
    }

    private void SetNameAndId(string name)
    {
        _configuration.Name = name;
        _configuration.Id = Guid.NewGuid();
    }
}