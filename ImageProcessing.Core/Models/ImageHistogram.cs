namespace Core.Models;

public class ImageHistogram
{
    public int[] Frequencies { get; }

    private ImageHistogram(int[] frequencies)
    {
        Frequencies = frequencies;
    }

    public static ImageHistogram FromImageData(ImageData imageData)
    {
        var frequencies = new int[256];

        foreach (var pixel in imageData.Pixels)
        {
            frequencies[pixel]++;
        }

        return new ImageHistogram(frequencies);
    }
    
    public double[] GetAccumulatedProbabilities()
    {
        var probabilities = new double[256];

        var pixelCount = Frequencies.Sum();
        var sum = 0;
        
        for (int i = 0; i < Frequencies.Length; i++)
        {
            sum += Frequencies[i];
            probabilities[i] = (double)sum / pixelCount;
        }

        return probabilities;
    }
}