using Core.Models;

namespace Core.Algorithms.Contrast;

public class InvertedContrastAlgorithm : IImageProcessingAlgorithm
{
    public ImageData Process(ImageData image)
    {
        var result = new byte[image.Pixels.Length];

        for (int i = 0; i < image.Pixels.Length; i++)
        {
            result[i] = (byte)(255 - image.Pixels[i]);
        }

        return new ImageData(image.Height, image.Width, result);
    }
}