namespace Persistence;

internal class StorageOptions
{
    public const string SectionName = "Storage";

    public const string DefaultBasePath = "/home/app/data";

    public string BasePath { get; set; } = DefaultBasePath;

    internal string DatabaseDirectory => Path.Combine(BasePath, "db");

    internal string DatabasePath => Path.Combine(DatabaseDirectory, "ThisIsYourLife.db");
}
