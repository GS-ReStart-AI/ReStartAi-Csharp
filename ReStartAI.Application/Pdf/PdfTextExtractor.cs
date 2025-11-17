using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace ReStartAI.Application.Pdf
{
    public interface IPdfTextExtractor
    {
        string ExtractText(Stream pdfStream);
    }

    public class PdfTextExtractor : IPdfTextExtractor
    {
        public string ExtractText(Stream pdfStream)
        {
            var sb = new StringBuilder();

            using (var document = PdfDocument.Open(pdfStream))
            {
                foreach (Page page in document.GetPages())
                {
                    sb.AppendLine(page.Text);
                }
            }

            return sb.ToString();
        }
    }
}