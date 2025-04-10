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
    private string participant = "";
    private string course = "";
    private DateTime date;
    private string fileName = "";
    private string participantRole = "";
    private bool isEmailChecked = false;
    private bool isSaveLocallyChecked = false;
    private string email = "";
    private string fullFilePath = "";
    private string folderPath = "";

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
                folderPath = result[0].Path.LocalPath;
                filePathMessage.Text = fullFilePath;
            }
        } catch (Exception err) {
            Console.WriteLine(err);
        }
    }

    private async void GeneratePdfButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            GatherInput();

            PdfDocument document = CreatePdf();

            document.Save(fullFilePath);

            if (isEmailChecked && SendEmailWithAttachment.ValidateRecipientEmailAddress(email))
            {
                await SendEmailWithAttachment.Send(email, participant, course, fullFilePath);
            }
            else if (isEmailChecked && !SendEmailWithAttachment.ValidateRecipientEmailAddress(email))
            {
                message.Text = "Missing or invalid email address. Please provide a valid email.";
            }

            if (!isSaveLocallyChecked)
            {
                File.Delete(fullFilePath);
                Console.WriteLine("File deleted successfully");
            }
            else
            {
                message.Text = $"File has been saved to {fullFilePath}";
            }



        }
        catch (Exception)
        {
            throw;
        }
    }

    private void GatherInput()
    {
        participant = participantName.Text ?? string.Empty;
        course = courseName.Text ?? string.Empty;
        date = completionDate.SelectedDate?.DateTime ?? DateTime.Today;
        fileName = $"{participant.Replace(" ", "_").ToLower()}_{course.Replace(" ", "_").ToLower()}_certificate.pdf";
        participantRole = "participant";
        isEmailChecked = sendEmail.IsChecked ?? false;
        isSaveLocallyChecked = saveLocally.IsChecked ?? false;
        email = studentEmail.Text ?? string.Empty;
        fullFilePath = Path.Combine(folderPath, fileName);
    }

    private PdfDocument CreatePdf()
    {
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

        return document;
    }
}
