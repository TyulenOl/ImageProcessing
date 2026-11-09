using Core.Models;

namespace Core.Algorithms.Contrast;

public class BrightnessSlicingAlgorithm : IImageProcessingAlgorithm
{
    private readonly byte minBrightness;
    private readonly byte maxBrightness;
    private readonly byte outsideValue;

    public BrightnessSlicingAlgorithm(byte minBrightness=byte.MinValue, byte maxBrightness=byte.MaxValue, byte outsideValue = 0)
    {
        if (minBrightness >= maxBrightness)
        {
            throw new ArgumentException("The minimum brightness must be less than the maximum.");
        }
        
        this.minBrightness = minBrightness;
        this.maxBrightness = maxBrightness;
        this.outsideValue = outsideValue;
    }

    public ImageData Process(ImageData image)
    {
        var result = new byte[image.Pixels.Length];

        for (int i = 0; i < image.Pixels.Length; i++)
        {
            var pixel = image.Pixels[i];
            result[i] = pixel >= minBrightness && pixel <= maxBrightness ? pixel : outsideValue;
        }

        return new ImageData(image.Height, image.Width, result);
    }
}