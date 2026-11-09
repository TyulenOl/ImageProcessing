using System;
using System.Collections.Generic;
using Core.Algorithms;
using Core.Algorithms.Contrast;
using Core.Algorithms.Histogram;

namespace SIOVI.Models;

public sealed class AlgorithmOption
{
    private readonly Func<IReadOnlyList<AlgorithmParameter>, IImageProcessingAlgorithm> create;

    public string Name { get; }
    public IReadOnlyList<AlgorithmParameter> Parameters { get; }

    private AlgorithmOption(string name, Func<IReadOnlyList<AlgorithmParameter>, IImageProcessingAlgorithm> create,
        params AlgorithmParameter[] parameters)
    {
        Name = name;
        this.create = create;
        Parameters = parameters;
    }

    public IImageProcessingAlgorithm CreateAlgorithm() => create(Parameters);

    public static IReadOnlyList<AlgorithmOption> CreateAll() => new[]
    {
        new AlgorithmOption(
            "Линейное контрастное масштабирование",
            p =>
            {
                var (min, max) = ReadRange(p);
                return new LinearContrastAlgorithm(min, max);
            },
            MinBrightness(),
            MaxBrightness()
        ),
        new AlgorithmOption(
            "Инвертированное контрастное масштабирование",
            _ => new InvertedContrastAlgorithm()
        ),
        new AlgorithmOption(
            "Яркостной срез",
            p =>
            {
                var (min, max) = ReadRange(p);
                return new BrightnessSlicingAlgorithm(min, max, p[2].ReadByte());
            },
            MinBrightness(),
            MaxBrightness(),
            new AlgorithmParameter(
                "Яркость вне диапазона",
                "0"
            )
        ),
        new AlgorithmOption(
            "Равномерная гистограмма",
            p =>
            {
                var (min, max) = ReadRange(p);
                return new UniformHistogramAlgorithm(min, max);
            },
            MinIntensity(),
            MaxIntensity()
        ),
        new AlgorithmOption(
            "Экспоненциальная гистограмма",
            p =>
            {
                var min = p[0].ReadByte();
                var alpha = p[1].ReadPositiveNumber();
                if (!double.IsFinite(1 / alpha))
                {
                    throw new ArgumentException("Коэффициент \"альфа\" слишком мал для вычисления.");
                }

                return new ExponentialHistogramAlgorithm(min, alpha);
            },
            MinIntensity(),
            Alpha("0.05")
        ),
        new AlgorithmOption(
            "Гистограмма Релея",
            p =>
            {
                var min = p[0].ReadByte();
                var alpha = p[1].ReadPositiveNumber();
                if (alpha * alpha == 0 || !double.IsFinite(2 * alpha * alpha))
                {
                    throw new ArgumentException("Коэффициент \"альфа\" слишком мал или велик для вычисления.");
                }

                return new RayleighHistogramAlgorithm(min, alpha);
            },
            MinIntensity(),
            Alpha("50")
        ),
        new AlgorithmOption(
            "Гистограмма степени 2/3",
            p =>
            {
                var (min, max) = ReadRange(p);
                return new PowerHistogramAlgorithm(min, max);
            },
            MinIntensity(),
            MaxIntensity()
        ),
        new AlgorithmOption(
            "Гиперболическая гистограмма",
            p =>
            {
                var (min, max) = ReadRange(p);
                if (min == 0)
                {
                    throw new ArgumentException("Минимальная яркость должна быть больше 0.");
                }

                return new HyperbolicHistogramAlgorithm(min, max);
            },
            new AlgorithmParameter("Минимальная яркость", "1", "Целое число от 1 до 254"),
            MaxIntensity()
        )
    };

    private static AlgorithmParameter MinBrightness() => new("Минимальная яркость", "0");
    private static AlgorithmParameter MaxBrightness() => new("Максимальная яркость", "255");
    
    private static AlgorithmParameter MinIntensity() => new("Минимальная интенсивность", "0");
    private static AlgorithmParameter MaxIntensity() => new("Максимальная интенсивность", "255");
    

    private static AlgorithmParameter Alpha(string value) =>
        new("Коэффициент \"альфа\"", value, "Число больше 0");

    private static (byte Min, byte Max) ReadRange(IReadOnlyList<AlgorithmParameter> parameters)
    {
        var min = parameters[0].ReadByte();
        var max = parameters[1].ReadByte();
        if (min >= max)
        {
            throw new ArgumentException("Минимальная яркость должна быть меньше максимальной.");
        }

        return (min, max);
    }
}