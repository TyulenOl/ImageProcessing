using System;
using System.Globalization;

namespace SIOVI.Models;

public sealed class AlgorithmParameter
{
    public string Name { get; }
    public string Hint { get; }
    public string Value { get; set; }

    public AlgorithmParameter(string name, string value, string hint = "Целое число от 0 до 255")
    {
        Name = name;
        Value = value;
        Hint = hint;
    }

    public byte ReadByte()
    {
        if (!byte.TryParse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            throw new ArgumentException($"«{Name}»: введите целое число от 0 до 255.");
        }

        return value;
    }

    public double ReadPositiveNumber()
    {
        if (!double.TryParse(Value.Trim().Replace(',', '.'), NumberStyles.Float,
                CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value) || value <= 0)
        {
            throw new ArgumentException($"«{Name}»: введите положительное конечное число.");
        }

        return value;
    }
}
