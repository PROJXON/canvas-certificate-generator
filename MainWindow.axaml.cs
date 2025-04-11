namespace CanvasCertificateGenerator;
using CanvasCertificateGenerator.Services;

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Fonts;

public partial class MainWindow : Window
{
    public record CertificateData
    (
        string Participant,
        string Course,
        DateTime Date,
        string ParticipantRole
    );

    private string participant = "";
    private string course = "";
    private string participantRole = "";
    private string email = "";
    private DateTime date;
    private bool isEmailChecked = false;
    private bool isSaveLocallyChecked = false;
    private string fileName = "";
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

    private void SaveLocallyCheckbox_OnChecked(object? sender, RoutedEventArgs e)
    {
        pdfDestinationLabel.IsVisible = true;
        pdfDestinationButton.IsVisible = true;
        filePathDisplayLabel.IsVisible = true;
        filePathMessage.IsVisible = true;
        Height = 500;
    }

    private void SaveLocallyCheckbox_OnUnChecked(object? sender, RoutedEventArgs e)
    {
        pdfDestinationLabel.IsVisible = false;
        pdfDestinationButton.IsVisible = false;
        filePathDisplayLabel.IsVisible = false;
        filePathMessage.IsVisible = false;
        Height = 450;
    }

    private async void GeneratePdfButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            message.Text = "";
            GatherInput();
            CertificateData data = new(participant, course, date, participantRole);
            PdfDocument pdf = CertificateService.CreatePdf(data);

            if (string.IsNullOrWhiteSpace(participant) || string.IsNullOrWhiteSpace(course) || !completionDate.SelectedDate.HasValue)
            {
                message.Text = "Please ensure that all required fields are filled out.";

                // TODO make borders of the required fields red when this happens

                return;
            }
            else
            {
                pdf.Save(fullFilePath);

                if (isEmailChecked && EmailService.ValidateRecipientEmailAddress(email))
                {
                    await EmailService.SendAsync(email, participant, course, fullFilePath);
                }
                else if (isEmailChecked && !EmailService.ValidateRecipientEmailAddress(email))
                {
                    message.Text = "Missing or invalid email address. Please provide a valid email.";
                }

                if (!isSaveLocallyChecked)
                {
                    File.Delete(fullFilePath);
                }
                else
                {
                    message.Text = $"File has been saved to {fullFilePath}";
                }
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
}
