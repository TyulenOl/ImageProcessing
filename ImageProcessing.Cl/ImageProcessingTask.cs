using Core.Algorithms;

namespace ImageProcessing.Cl;

public class ImageProcessingTask
{
    public IImageProcessingAlgorithm Algorithm { get; }
    public string InputPath { get; }
    public string OutputPath { get; }
    public string HistogramOutputPath { get; }
    
    public ImageProcessingTask(IImageProcessingAlgorithm algorithm, string inputPath, string outputPath, string histogramOutputPath)
    {
        Algorithm = algorithm;
        InputPath = inputPath;
        OutputPath = outputPath;
        HistogramOutputPath = histogramOutputPath;
    }
}