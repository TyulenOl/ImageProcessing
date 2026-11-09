using Core.Models;

namespace Core.Algorithms.Contrast;

public class LinearContrastAlgorithm : IImageProcessingAlgorithm
{
    private readonly byte minBrightness;
    private readonly byte maxBrightness;

    public LinearContrastAlgorithm(byte minBrightness=byte.MinValue, byte maxBrightness=byte.MaxValue)
    {
        if (minBrightness >= maxBrightness)
        {
            throw new ArgumentException("The minimum brightness must be less than the maximum.");
        }
        
        this.minBrightness = minBrightness;
        this.maxBrightness = maxBrightness;
    }

    public ImageData Process(ImageData image)
    {
        var result = new byte[image.Pixels.Length];
        var scaleFactor = (maxBrightness - minBrightness) / 255.0;

        for (int i = 0; i < image.Pixels.Length; i++)
        {
            var value = scaleFactor * image.Pixels[i] + minBrightness;
            result[i] = (byte)Math.Round(value);
        }

        return new ImageData(image.Height, image.Width, result);
    }
}