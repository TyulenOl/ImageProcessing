using Core.Models;

namespace Core.Algorithms;

public interface IImageProcessingAlgorithm
{
    ImageData Process(ImageData image);
}