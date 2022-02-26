using System.IO;
using System.Threading.Tasks;

namespace Persistence;

public interface IFileSystem
{
    public Task CreateFileAsync(string filePath, Stream content);

    void DeleteFile(string filePath);

    Stream OpenRead(string filePath);

    void WriteAllText(string path, string contents);

    bool DirectoryExists(string path);

    void CreateDirectory(string path);

    bool FileExists(string path);
}