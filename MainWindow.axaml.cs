namespace CanvasCertificateGenerator;

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

public partial class MainWindow : Window
{
    private string path = "";
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void FileDestinationButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var options = new FolderPickerOpenOptions
            {
                Title = "Choose Certificate Folder Destination",
            };

            var result = await StorageProvider.OpenFolderPickerAsync(options);

            Console.WriteLine(path);

            if (result is not null)
            {

            }
        } catch (Exception err) {
            Console.WriteLine(err);
        }
    }

    private async void GeneratePdfButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {

        }
        catch (Exception)
        {

            throw;
        }
    }
}
