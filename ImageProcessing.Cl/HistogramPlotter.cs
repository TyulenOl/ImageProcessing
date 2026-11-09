using Core.Models;
using ScottPlot;

namespace ImageProcessing.Cl;

public class HistogramPlotter
{
    public void Save(ImageHistogram histogram, string path)
    {
        var plot = new Plot();

        var frequencies = histogram.Frequencies.Select(x => (double)x).ToArray();

        var barPlot = plot.Add.Bars(frequencies);
        
        foreach (var bar in barPlot.Bars)
        {
            bar.Size = 1;
            bar.LineWidth = 0;
            bar.FillStyle.AntiAlias = false;
        }
        
        plot.Axes.Margins(bottom: 0);
        plot.Axes.SetLimitsX(0, 255);
        plot.YLabel("Количество пикселей");
        plot.XLabel("Яркость");

        plot.SavePng(path, 1200, 600);
    }
}