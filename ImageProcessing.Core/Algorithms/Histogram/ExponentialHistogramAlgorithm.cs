using Core.Models;

namespace Core.Algorithms.Histogram;

public class ExponentialHistogramAlgorithm : IImageProcessingAlgorithm
{
    private readonly byte minIntensity;
    private readonly double alpha;

    public ExponentialHistogramAlgorithm(byte minIntensity=byte.MinValue, double alpha=0.05)
    {
        this.minIntensity = minIntensity;
        this.alpha = alpha;
    }
    
    public ImageData Process(ImageData image)
    {
        var result = new byte[image.Pixels.Length];
        
        var histogram = ImageHistogram.FromImageData(image);
        var accumPr = histogram.GetAccumulatedProbabilities();

        for (int i = 0; i < image.Pixels.Length; i++)
        {
            var pixel = image.Pixels[i];
            var value = minIntensity - ((1 / alpha) * Math.Log(1 - accumPr[pixel]));
            result[i] = (byte)Math.Clamp(Math.Round(value), 0, 255);
        }

        return new ImageData(image.Height, image.Width, result);
    }
}