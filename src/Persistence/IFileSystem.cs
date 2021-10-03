using System.IO;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IFileSystem
    {
        public Task CreateFileAsync(string filePath, Stream content);
    }
}