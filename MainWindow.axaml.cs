namespace CanvasCertificateGenerator;

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

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
                filePathMessage.Text = path;
            }
        } catch (Exception err) {
            Console.WriteLine(err);
        }
    }

    private async void GeneratePdfButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            string participant = participantName.Text ?? string.Empty;
            string course = courseName.Text ?? string.Empty;
            DateTime date = completionDate.SelectedDate?.DateTime ?? DateTime.Today;
            var dateStr = date.ToShortDateString();
            string filename = $"{participant.Replace(" ", "_").ToLower()}{course.Replace(" ", "_").ToLower()}_tempfile.pdf";

            // set up PdfSharp document and page
            var document = new PdfDocument();
            var page = document.AddPage();
            page.Width = XUnit.FromPoint(2000);
            page.Height = XUnit.FromPoint(1545);

            var gfx = XGraphics.FromPdfPage(page);

            // loads background image
            var background = XImage.FromFile("./Template.png");
            gfx.DrawImage(background, 0, 0, page.Width, page.Height);



            document.Save(filename);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
