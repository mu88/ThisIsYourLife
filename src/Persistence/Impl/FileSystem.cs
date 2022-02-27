using System.IO;

namespace Persistence;

internal class FileSystem : IFileSystem
{
    public Stream CreateFile(string filePath) => File.Create(filePath);

    /// <inheritdoc />
    public void DeleteFile(string filePath) => File.Delete(filePath);

    /// <inheritdoc />
    public Stream OpenRead(string filePath) => File.OpenRead(filePath);

    /// <inheritdoc />
    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

    /// <inheritdoc />
    public bool DirectoryExists(string path) => Directory.Exists(path);

    /// <inheritdoc />
    public bool DirectoryExists(DirectoryInfo directoryInfo) => directoryInfo.Exists;

    /// <inheritdoc />
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    /// <inheritdoc />
    public bool FileExists(string path) => File.Exists(path);
}