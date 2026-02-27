namespace Persistence;

public interface IFileSystem
{
    void DeleteFile(string filePath);

    Stream OpenRead(string filePath);

    void WriteAllText(string path, string contents);

    bool DirectoryExists(string path);

    bool DirectoryExists(DirectoryInfo directoryInfo);

    void CreateDirectory(string path);

    bool FileExists(string path);

    Stream CreateFile(string filePath);
}
