using System.IO;

namespace DTO.LifePoint;

public record ImageToCreate(string FileName, Stream Stream)
{
}