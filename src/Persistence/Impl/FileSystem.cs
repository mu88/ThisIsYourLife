using System;
using System.IO;
using System.Threading.Tasks;

namespace Persistence;

internal class FileSystem : IFileSystem
{
    /// <inheritdoc />
    public async Task CreateFileAsync(string filePath, Stream content)
    {
        var parentDirectory = Directory.GetParent(filePath);
        if (parentDirectory is not { Exists: true })
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException($"Could not parse '{filePath}'"));

        await using FileStream fileStream = new(filePath, FileMode.Create);
        await content.CopyToAsync(fileStream);
    }

    /// <inheritdoc />
    public void DeleteFile(string filePath) => File.Delete(filePath);

    /// <inheritdoc />
    public Stream OpenRead(string filePath) => File.OpenRead(filePath);

    /// <inheritdoc />
    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

    /// <inheritdoc />
    public bool DirectoryExists(string path) => Directory.Exists(path);

    /// <inheritdoc />
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    /// <inheritdoc />
    public bool FileExists(string path) => File.Exists(path);
}