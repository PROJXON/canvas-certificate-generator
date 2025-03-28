namespace CanvasCertificateGenerator;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void FileDestinationButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (StorageProvider is not null)
        {
            var options = new FolderPickerOpenOptions
            {
                Title = "Choose Certificate Folder Destination",
            };

            var result = await StorageProvider.OpenFolderPickerAsync(options);

            if (result is not null)
            {

            }
        }
    }
}
