namespace CanvasCertificateGenerator.Services;

using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

public class CertificateService
{
    public static PdfDocument CreatePdf(MainWindow.CertificateData data)
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        page.Width = XUnit.FromPoint(2000);
        page.Height = XUnit.FromPoint(1545);
        using var gfx = XGraphics.FromPdfPage(page);

        using var background = XImage.FromFile("./assets/Template.png");
        gfx.DrawImage(background, 0, 0, page.Width, page.Height);

        var courseFont = new XFont("Geologica", 52, XFontStyle.Bold);
        var nameFont = new XFont("Geologica", 65, XFontStyle.Bold);
        var smallFont = new XFont("Roboto", 26, XFontStyle.Regular);
        var dateFont = new XFont("Roboto", 24, XFontStyle.Regular);
        var whiteBrush = XBrushes.White;
        var yellowBrush = new XSolidBrush(XColor.FromArgb(255, 215, 0));

        gfx.DrawString(data.Participant, nameFont, whiteBrush, new XPoint(1220, 980), XStringFormats.Center);
        gfx.DrawString(data.Course.ToUpper(), courseFont, yellowBrush, new XPoint(1220, 485), XStringFormats.Center);
        gfx.DrawString(data.Date.ToShortDateString(), dateFont, whiteBrush, new XPoint(1475, 1410), XStringFormats.Center);
        gfx.DrawString($"This Certificate is presented to {data.Participant} for their outstanding", smallFont, whiteBrush, new XPoint(1220, 1130), XStringFormats.Center);
        gfx.DrawString($"completion of the {data.Course} course (as a {data.ParticipantRole}).", smallFont, whiteBrush, new XPoint(1220, 1170), XStringFormats.Center);

        return document;
    }
}
