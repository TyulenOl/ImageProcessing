using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Core.Models;
using Microsoft.Win32;
using ScottPlot.WPF;
using SIOVI.Models;
using SIOVI.Services;

namespace SIOVI;

public partial class MainWindow : Window
{
    private ImageData? originalImage;
    private bool isBusy;

    public MainWindow()
    {
        InitializeComponent();
        ResetHistogram(OriginalHistogram);
        ResetHistogram(ProcessedHistogram);
        AlgorithmSelector.ItemsSource = AlgorithmOption.CreateAll();
        AlgorithmSelector.SelectedIndex = 0;
    }

    private async void OpenImage_Click(object sender, RoutedEventArgs e)
    {
        if (isBusy)
        {
            return;
        }

        var dialog = new OpenFileDialog
        {
            Title = "Выберите изображение",
            Filter = "Изображения|*.png;*.jpg;*.jpeg|Все файлы|*.*",
            CheckFileExists = true,
            Multiselect = false
        };
        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        SetBusy(true);
        ErrorText.Text = string.Empty;
        
        try
        {
            var loaded = await Task.Run(() =>
            {
                var image = ImageFileService.LoadGrayscale(dialog.FileName);
                return (Image: image, Bitmap: ImageFileService.CreateBitmap(image),
                    Histogram: ImageHistogram.FromImageData(image));
            });

            originalImage = loaded.Image;
            OriginalImage.Source = loaded.Bitmap;
            DrawHistogram(OriginalHistogram, loaded.Histogram);
            FileNameText.Text = dialog.FileName;
            ClearResult();
        }
        catch (Exception ex)
        {
            ErrorText.Text = $"Не удалось открыть изображение: {ex.Message}";
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void AlgorithmSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AlgorithmSelector.SelectedItem is not AlgorithmOption option)
        {
            return;
        }

        ParametersList.ItemsSource = option.Parameters;
        NoParametersText.Visibility = option.Parameters.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        InvalidateResult();
    }

    private void ParameterInput_TextChanged(object sender, TextChangedEventArgs e) => InvalidateResult();

    private void InvalidateResult()
    {
        ClearResult();
        ErrorText.Text = string.Empty;
    }

    private async void Apply_Click(object sender, RoutedEventArgs e)
    {
        if (isBusy || originalImage == null || AlgorithmSelector.SelectedItem is not AlgorithmOption option)
        {
            return;
        }

        ErrorText.Text = string.Empty;
        try
        {
            var algorithm = option.CreateAlgorithm();
            var source = originalImage;
            SetBusy(true);

            var processed = await Task.Run(() =>
            {
                var result = algorithm.Process(source);
                return (Bitmap: ImageFileService.CreateBitmap(result), Histogram: ImageHistogram.FromImageData(result));
            });

            ProcessedImage.Source = processed.Bitmap;
            DrawHistogram(ProcessedHistogram, processed.Histogram);
        }
        catch (ArgumentException ex)
        {
            ErrorText.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorText.Text = $"Не удалось обработать изображение: {ex.Message}";
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        isBusy = busy;
        ControlsPanel.IsEnabled = !busy;
        ApplyButton.IsEnabled = !busy && originalImage != null && AlgorithmSelector.SelectedItem != null;
    }

    private void ClearResult()
    {
        ProcessedImage.Source = null;
        ResetHistogram(ProcessedHistogram);
    }

    private static void ResetHistogram(WpfPlot control)
    {
        control.Plot.Clear();
        control.Plot.XLabel("Яркость");
        control.Plot.YLabel("Количество пикселей");
        control.Plot.Axes.SetLimits(-0.5, 255.5, 0, 1);
        control.Refresh();
    }

    private static void DrawHistogram(WpfPlot control, ImageHistogram histogram)
    {
        control.Plot.Clear();
        var frequencies = histogram.Frequencies.Select(value => (double)value).ToArray();
        var bars = control.Plot.Add.Bars(frequencies);
        foreach (var bar in bars.Bars)
        {
            bar.Size = 1;
            bar.LineWidth = 0;
        }
        
        control.Plot.Axes.SetLimits(-0.5, 255.5, 0, Math.Max(1, frequencies.Max() * 1.05));
        control.Refresh();
    }
}
