using System.IO;
using DTO.LifePoint;

namespace Tests.Doubles;

public static class TestImageToCreate
{
    public static ImageToCreate Create()
    {
        var destFileName = Path.GetTempFileName();
        File.Copy("Doubles/Dynamo.jpg", destFileName, true);
        return new ImageToCreate(new FileStream(destFileName, FileMode.Open));
    }
}