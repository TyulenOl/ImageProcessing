using Core.Algorithms.Contrast;
using Core.Algorithms.Histogram;
using Core.Models;
using ImageProcessing.Cl;

var imageFolder = "D:\\Tyulenev\\c#\\SIOVI\\ImageProcessing.Cl\\Images";
var histogramFolder = "D:\\Tyulenev\\c#\\SIOVI\\ImageProcessing.Cl\\Histograms";

var imageOriginFolder = Path.Combine(imageFolder, "Origin");
var imageProcessedFolder = Path.Combine(imageFolder, "Processed");
var inputPath = Path.Combine(imageOriginFolder, "images.jpg");

var tasks = new List<ImageProcessingTask>
{
    new(
        new LinearContrastAlgorithm(0, 150),
        inputPath,
        Path.Combine(imageProcessedFolder, "linear.png"),
        Path.Combine(histogramFolder, "linear.png")
    ),
    new(
        new InvertedContrastAlgorithm(),
        inputPath,
        Path.Combine(imageProcessedFolder, "inverted.png"),
        Path.Combine(histogramFolder, "inverted.png")
    ),
    new(
        new BrightnessSlicingAlgorithm(80, 180),
        inputPath,
        Path.Combine(imageProcessedFolder, "slicing.png"),
        Path.Combine(histogramFolder, "slicing.png")
    ),
    new(
        new UniformHistogramAlgorithm(),
        inputPath,
        Path.Combine(imageProcessedFolder, "uniform.png"),
        Path.Combine(histogramFolder, "uniform.png")
    ),
    new(
        new ExponentialHistogramAlgorithm(alpha:0.02),
        inputPath,
        Path.Combine(imageProcessedFolder, "exponential.png"),
        Path.Combine(histogramFolder, "exponential.png")
    ),
    new(
        new RayleighHistogramAlgorithm(alpha:70),
        inputPath,
        Path.Combine(imageProcessedFolder, "rayleigh.png"),
        Path.Combine(histogramFolder, "rayleigh.png")
    ),
    new(
        new PowerHistogramAlgorithm(),
        inputPath,
        Path.Combine(imageProcessedFolder, "power.png"),
        Path.Combine(histogramFolder, "power.png")
    ),
    new(
        new HyperbolicHistogramAlgorithm(),
        inputPath,
        Path.Combine(imageProcessedFolder, "hyperbolic.png"),
        Path.Combine(histogramFolder, "hyperbolic.png")
    )
};

var imageFileService = new ImageFileService();
var histogramPlotter = new HistogramPlotter();

var uniqueImagePaths = tasks.Select(x => x.InputPath).ToHashSet();
foreach (var imagePath in uniqueImagePaths)
{
    var imageData = imageFileService.LoadGrayscale(imagePath);
    var histogram = ImageHistogram.FromImageData(imageData);
    var fileName = Path.GetFileNameWithoutExtension(imagePath) + ".png";
    histogramPlotter.Save(histogram, Path.Combine(histogramFolder, fileName));
}

foreach (var task in tasks)
{
    var imageData = imageFileService.LoadGrayscale(task.InputPath);
    
    var processedImageData = task.Algorithm.Process(imageData);
    imageFileService.SaveGrayscale(processedImageData, task.OutputPath);

    var histogram = ImageHistogram.FromImageData(processedImageData);
    histogramPlotter.Save(histogram, task.HistogramOutputPath);
}