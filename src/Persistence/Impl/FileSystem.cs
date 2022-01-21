using System.IO;
using System.Threading.Tasks;

namespace Persistence;

internal class FileSystem : IFileSystem
{
    /// <inheritdoc />
    public async Task CreateFileAsync(string filePath, Stream content)
    {
        await using FileStream fileStream = new(filePath, FileMode.Create);
        await content.CopyToAsync(fileStream);
    }
}