namespace ImageProcessing.Cl;

public class ImageResultPaths
{
    private readonly string imageDirectory;
    private readonly string histogramDirectory;
    
    public ImageResultPaths(string rootDirectory, string imageName)
    {
        var directory = Path.Combine(rootDirectory, imageName);
        
        imageDirectory = Path.Combine(directory, "Images");
        histogramDirectory = Path.Combine(directory, "Histograms");

        Directory.CreateDirectory(directory);
        Directory.CreateDirectory(imageDirectory);
        Directory.CreateDirectory(histogramDirectory);
    }

    public string GetImagePath(string name)
    {
        return Path.Combine(imageDirectory, $"image_{name}.png");
    }
    
    public string GetHistogramPath(string name)
    {
        return Path.Combine(histogramDirectory, $"histogram_{name}.png");
    }
}