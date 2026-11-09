using Core.Models;

namespace Core.Algorithms.Histogram;

public class HyperbolicHistogramAlgorithm : IImageProcessingAlgorithm
{
    private readonly byte minIntensity;
    private readonly byte maxIntensity;

    public HyperbolicHistogramAlgorithm(byte minIntensity=1, byte maxIntensity=byte.MaxValue)
    {
        if (minIntensity <= 0)
        {
            throw new ArgumentException("The minimum intensity must be greater than 0.");
        }
        
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

        for (int i = 0; i < image.Pixels.Length; i++)
        {
            var pixel = image.Pixels[i];
            var value = minIntensity * Math.Pow((double)maxIntensity / minIntensity, accumPr[pixel]);
            result[i] = (byte)Math.Clamp(Math.Round(value), 0, 255);
        }

        return new ImageData(image.Height, image.Width, result);
    }
}