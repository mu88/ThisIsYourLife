using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Logging.Extensions;

public static partial class Log
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Starting method `{methodName}`")]
    public static partial void MethodStarted(this ILogger logger, [CallerMemberName] string methodName = "");

    [LoggerMessage(EventId = 100, Level = LogLevel.Debug, Message = "Method `{methodName}` finished")]
    public static partial void MethodFinished(this ILogger logger, [CallerMemberName] string methodName = "");

    [LoggerMessage(EventId = 1000, Level = LogLevel.Debug, Message = "Setting username to `{username}`")]
    public static partial void SettingUser(this ILogger logger, string username);

    [LoggerMessage(EventId = 1100, Level = LogLevel.Debug, Message = "Username set to `{username}`")]
    public static partial void UserSet(this ILogger logger, string username);

    [LoggerMessage(EventId = 1200, Level = LogLevel.Information, Message = "Creating database directory `{databaseDirectory}`")]
    public static partial void CreatingDatabaseDirectory(this ILogger logger, string databaseDirectory);

    [LoggerMessage(EventId = 1300, Level = LogLevel.Information, Message = "Creating database `{databasePath}`")]
    public static partial void CreatingDatabase(this ILogger logger, string databasePath);

    [LoggerMessage(EventId = 1400, Level = LogLevel.Information, Message = "Database `{databasePath}` created and migrations executed")]
    public static partial void DatabaseCreated(this ILogger logger, string databasePath);

    [LoggerMessage(EventId = 1500, Level = LogLevel.Debug, Message = "Initializing database with seeded data")]
    public static partial void SeedingData(this ILogger logger);

    [LoggerMessage(EventId = 1600, Level = LogLevel.Debug, Message = "Executing OnModelCreating() from base class")]
    public static partial void ExecutingBaseOnModelCreating(this ILogger logger);

    [LoggerMessage(EventId = 1700, Level = LogLevel.Debug, Message = "Executing custom OnModelCreating() logic")]
    public static partial void ExecutingCustomOnModelCreating(this ILogger logger);

    [LoggerMessage(EventId = 1800, Level = LogLevel.Information, Message = "Image directory `{imageDirectory}` created")]
    public static partial void ImageDirectoryCreated(this ILogger logger, DirectoryInfo imageDirectory);

    [LoggerMessage(EventId = 1900, Level = LogLevel.Debug, Message = "Image `{imageId}` resized and saved")]
    public static partial void ImageResizedAndSaved(this ILogger logger, Guid imageId);

    [LoggerMessage(EventId = 2000, Level = LogLevel.Debug, Message = "Creating new LifePoint")]
    public static partial void CreatingLifePoint(this ILogger logger);

    [LoggerMessage(EventId = 2100, Level = LogLevel.Debug, Message = "Extracted new LifePoint from DTO")]
    public static partial void NewLifePointExtracted(this ILogger logger);

    [LoggerMessage(EventId = 2200, Level = LogLevel.Information, Message = "Created new LifePoint with ID '{idOfCreatedLifePoint}'")]
    public static partial void NewLifePointCreated(this ILogger logger, Guid idOfCreatedLifePoint);

    [LoggerMessage(EventId = 2300, Level = LogLevel.Debug, Message = "Image ID '{imageId}' of owner ID '{ownerId}' deleted")]
    public static partial void ImageDeleted(this ILogger logger, Guid ownerId, Guid imageId);

    [LoggerMessage(EventId = 2400, Level = LogLevel.Information, Message = "LifePoint with ID '{idOfDeletedLifePoint}' deleted")]
    public static partial void LifePointDeleted(this ILogger logger, Guid idOfDeletedLifePoint);

    [LoggerMessage(EventId = 2500, Level = LogLevel.Information, Message = "Created new Person with ID '{idOfCreatedPerson}'")]
    public static partial void NewPersonCreated(this ILogger logger, Guid idOfCreatedPerson);

    [LoggerMessage(EventId = 2600, Level = LogLevel.Warning, Message = "There was a too big image'")]
    public static partial void ImageTooBig(this ILogger logger);
}