using DTO.LifePoint;
using SkiaSharp;

namespace Tests.Doubles;

public static class TestImageToCreate
{
    public static ImageToCreate Create(MemoryStream? imageMemoryStream = null)
    {
        if (imageMemoryStream != null)
        {
            return new ImageToCreate(imageMemoryStream);
        }

        var destFileName = Path.GetTempFileName();
        File.Copy("Doubles/Dynamo.jpg", destFileName, true);
        return new ImageToCreate(new FileStream(destFileName, FileMode.Open));
    }

    public static ImageToCreate CreateWithSize(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.DodgerBlue);

        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(SKEncodedImageFormat.Jpeg, 90);
        var stream = new MemoryStream(encoded.ToArray());
        stream.Position = 0;
        return new ImageToCreate(stream);
    }
}
