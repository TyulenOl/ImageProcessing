using Core.Algorithms;
using Core.Algorithms.Contrast;
using Core.Algorithms.Histogram;
using Core.Models;
using ImageProcessing.Cl;

var originDirectory = "D:\\Tyulenev\\c#\\SIOVI\\ImageProcessing.Cl\\Origin";
var resultsDirectory = "D:\\Tyulenev\\c#\\SIOVI\\ImageProcessing.Cl\\Results";

var originImagesDirectory = Path.Combine(originDirectory, "Images");
var originHistogramsDirectory = Path.Combine(originDirectory, "Histograms");

var mediumImagePath = Path.Combine(originImagesDirectory, "medium_grayscale.png");
var lightImagePath = Path.Combine(originImagesDirectory, "light_grayscale.jpg");
var darkImagePath = Path.Combine(originImagesDirectory, "dark_grayscale.jpg");

var mediumPaths = new ImageResultPaths(resultsDirectory, "Medium");
var lightPaths = new ImageResultPaths(resultsDirectory, "Light");
var darkPaths = new ImageResultPaths(resultsDirectory, "Dark");

Directory.CreateDirectory(resultsDirectory);
Directory.CreateDirectory(originHistogramsDirectory);

var algorithms = new List<(string Name, IImageProcessingAlgorithm Algorithm)>
{
    ("linear", new LinearContrastAlgorithm(50, 200)),
    ("inverted", new InvertedContrastAlgorithm()),
    ("slicing", new BrightnessSlicingAlgorithm(50, 200)),
    ("uniform", new UniformHistogramAlgorithm()),
    ("exponential", new ExponentialHistogramAlgorithm(alpha: 0.02)),
    ("rayleigh", new RayleighHistogramAlgorithm(alpha: 70)),
    ("power", new PowerHistogramAlgorithm()),
    ("hyperbolic", new HyperbolicHistogramAlgorithm())
};

var images = new[]
{
    (Path: mediumImagePath, Results: mediumPaths),
    (Path: lightImagePath, Results: lightPaths),
    (Path: darkImagePath, Results: darkPaths)
};

var tasks = images.SelectMany(image =>
    algorithms.Select(algorithm =>
        new ImageProcessingTask(
            algorithm: algorithm.Algorithm,
            inputPath: image.Path,
            outputPath: image.Results.GetImagePath(algorithm.Name),
            histogramOutputPath: image.Results.GetHistogramPath(algorithm.Name)
        )
    )
).ToList();

var imageFileService = new ImageFileService();
var histogramPlotter = new HistogramPlotter();

foreach (var image in images)
{
    var imageData = imageFileService.LoadGrayscale(image.Path);
    var histogram = ImageHistogram.FromImageData(imageData);
    var fileName = "histogram_" + Path.GetFileNameWithoutExtension(image.Path) + ".png";
    histogramPlotter.Save(histogram, Path.Combine(originHistogramsDirectory, fileName));
}

foreach (var task in tasks)
{
    var imageData = imageFileService.LoadGrayscale(task.InputPath);

    var processedImageData = task.Algorithm.Process(imageData);
    imageFileService.SaveGrayscale(processedImageData, task.OutputPath);

    var histogram = ImageHistogram.FromImageData(processedImageData);
    histogramPlotter.Save(histogram, task.HistogramOutputPath);
}