namespace CanvasCertificateGenerator;
using CanvasCertificateGenerator.Services;

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;

public partial class MainWindow : Window
{
    private string path = "";
    public MainWindow()
    {
        InitializeComponent();

        // Register the custom font resolver
        GlobalFontSettings.FontResolver = new CustomFontResolver();
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
            string filename = $"{participant.Replace(" ", "_").ToLower()}_{course.Replace(" ", "_").ToLower()}_certificate.pdf";
            string participantRole = "participant";
            var isEmailChecked = sendEmail.IsChecked ?? false;
            string email = studentEmail.Text ?? string.Empty;

            var document = new PdfDocument();
            var page = document.AddPage();
            page.Width = XUnit.FromPoint(2000);
            page.Height = XUnit.FromPoint(1545);
            var gfx = XGraphics.FromPdfPage(page);

            var background = XImage.FromFile("./assets/Template.png");
            gfx.DrawImage(background, 0, 0, page.Width, page.Height);

            var courseFont = new XFont("Geologica", 52, XFontStyle.Bold);
            var nameFont = new XFont("Geologica", 65, XFontStyle.Bold);
            var smallFont = new XFont("Roboto", 26, XFontStyle.Regular);
            var dateFont = new XFont("Roboto", 24, XFontStyle.Regular);
            var whiteBrush = XBrushes.White;
            var yellowBrush = new XSolidBrush(XColor.FromArgb(255, 215, 0));

            gfx.DrawString(participant, nameFont, whiteBrush, new XPoint(1220, 980), XStringFormats.Center);
            gfx.DrawString(course.ToUpper(), courseFont, yellowBrush, new XPoint(1220, 485), XStringFormats.Center);
            gfx.DrawString(date.ToShortDateString(), dateFont, whiteBrush, new XPoint(1475, 1410), XStringFormats.Center);
            gfx.DrawString($"This Certificate is presented to {participant} for their outstanding", smallFont, whiteBrush, new XPoint(1220, 1130), XStringFormats.Center);
            gfx.DrawString($"completion of the {course} course (as a {participantRole}).", smallFont, whiteBrush, new XPoint(1220, 1170), XStringFormats.Center);

            document.Save(Path.Combine(path, filename));

            if (isEmailChecked && SendEmailWithAttachment.ValidateEmail(email))
            {
                await SendEmailWithAttachment.Send(email);
            }

        }
        catch (Exception)
        {
            throw;
        }
    }
}
