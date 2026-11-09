using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Core.Models;

namespace SIOVI.Services;

public static class ImageFileService
{
    public static ImageData LoadGrayscale(string path)
    {
        using var stream = File.OpenRead(path);
        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        var grayscale = new FormatConvertedBitmap(decoder.Frames[0], PixelFormats.Gray8, null, 0);
        var pixels = new byte[checked(grayscale.PixelWidth * grayscale.PixelHeight)];
        grayscale.CopyPixels(pixels, grayscale.PixelWidth, 0);
        return new ImageData(grayscale.PixelHeight, grayscale.PixelWidth, pixels);
    }

    public static BitmapSource CreateBitmap(ImageData image)
    {
        var bitmap = BitmapSource.Create(image.Width, image.Height, 96, 96,
            PixelFormats.Gray8, null, image.Pixels, image.Width);
        bitmap.Freeze();
        return bitmap;
    }
}
