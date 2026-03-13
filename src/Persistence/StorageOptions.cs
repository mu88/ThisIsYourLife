namespace Persistence;

public class StorageOptions
{
    public const string SectionName = "Storage";

    public const string DefaultBasePath = "/home/app/data";

    public string BasePath { get; set; } = DefaultBasePath;
}
