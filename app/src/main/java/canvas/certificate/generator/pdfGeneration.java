package canvas.certificate.generator;

// import com.itextpdf.kernel.colors.ColorConstants;
import com.itextpdf.kernel.font.PdfFont;
import com.itextpdf.kernel.font.PdfFontFactory;
import com.itextpdf.kernel.pdf.PdfDocument;
import com.itextpdf.kernel.pdf.PdfWriter;
import com.itextpdf.layout.Document;
import com.itextpdf.layout.element.Paragraph;
import com.itextpdf.layout.properties.TextAlignment;

import java.io.File;
import java.io.IOException;

public class pdfGeneration {
    public static void main(String[] args) {
        String destination = "certificate.pdf";
        String studentName = "John Doe";
        String courseTitle = "Java Programming";
        String CompletionDate = "01/01/2021";

        App.launch(args);

        // generateCertificate(destination, studentName, courseTitle, CompletionDate);
    }

    public static void generateCertificate(String destination, String studentName, String courseTitle,
            String completionDate) {
        try {
            PdfWriter writer = new PdfWriter(destination);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            PdfFont font = PdfFontFactory.createFont();

            Paragraph headingParagraph = new Paragraph(courseTitle)
                    .setFont(font)
                    .setFontSize(24)
                    .setBold()
                    .setTextAlignment(TextAlignment.CENTER);

            Paragraph nameParagraph = new Paragraph(studentName)
                    .setFont(font)
                    .setFontSize(20)
                    .setBold();

            Paragraph dateParagraph = new Paragraph(completionDate)
                    .setFont(font)
                    .setFontSize(20)
                    .setBold();

            document.add(headingParagraph);
            document.add(new Paragraph("\n"));
            document.add(nameParagraph);
            document.add(new Paragraph("\n"));
            document.add(dateParagraph);

            document.close();
            System.out.println("Certificate successfully generated at " + new File(destination).getAbsolutePath());
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
