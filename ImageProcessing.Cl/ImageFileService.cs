using Core.Models;
using ImageMagick;

namespace ImageProcessing.Cl;

public class ImageFileService
{
    public ImageData LoadGrayscale(string path)
    {
        using var image = new MagickImage(path);

        if (image.DetermineColorType() != ColorType.Grayscale)
        {
            throw new ArgumentException("The image must be grayscale.");
        }

        if (image.Depth != 8)
        {
            throw new ArgumentException("The image must be 8-bit.");
        }

        var height = (int)image.Height;
        var width = (int)image.Width;

        var pixels = new byte[height * width];

        var pixelCollection = image.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[y * width + x] = pixelCollection.GetPixel(x, y).GetChannel(0);
            }
        }

        return new ImageData(height, width, pixels);
    } 
    
    public void SaveGrayscale(ImageData imageData, string path)
    {
        var settings = new PixelReadSettings((uint)imageData.Width, (uint)imageData.Height, StorageType.Char, "I");

        using var image = new MagickImage();
        image.ReadPixels(imageData.Pixels, settings);
        image.Format = MagickFormat.Png;
        image.Write(path);
    } 
}