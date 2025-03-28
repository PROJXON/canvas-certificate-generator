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

            // Opens a dialog and allows the user to select a destination folder
            var result = await StorageProvider.OpenFolderPickerAsync(options);

            if (result != null && result.Count > 0)
            {
                // sets the selected folder as the path variable
                path = result[0].Path.LocalPath;
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
