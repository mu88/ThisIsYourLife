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
}