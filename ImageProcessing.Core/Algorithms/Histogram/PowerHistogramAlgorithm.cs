using Core.Models;

namespace Core.Algorithms.Histogram;

public class PowerHistogramAlgorithm : IImageProcessingAlgorithm
{
    private readonly byte minIntensity;
    private readonly byte maxIntensity;

    public PowerHistogramAlgorithm(byte minIntensity=byte.MinValue, byte maxIntensity=byte.MaxValue)
    {
        if (minIntensity >= maxIntensity)
        {
            throw new ArgumentException("The minimum intensity must be less than the maximum.");
        }
        
        this.minIntensity = minIntensity;
        this.maxIntensity = maxIntensity;
    }
    
    public ImageData Process(ImageData image)
    {
        var result = new byte[image.Pixels.Length];
        
        var histogram = ImageHistogram.FromImageData(image);
        var accumPr = histogram.GetAccumulatedProbabilities();

        var cbrtMinInt = Math.Cbrt(minIntensity);
        var cbrtMaxInt = Math.Cbrt(maxIntensity);
        
        for (int i = 0; i < image.Pixels.Length; i++)
        {
            var pixel = image.Pixels[i];
            var value = Math.Pow((cbrtMaxInt - cbrtMinInt) * accumPr[pixel] + cbrtMinInt, 3);
            result[i] = (byte)Math.Clamp(Math.Round(value), 0, 255);
        }

        return new ImageData(image.Height, image.Width, result);
    }
}