using AttivaMente.Core.Models;
using OfficeOpenXml.Style;
using System.Text.RegularExpressions;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace AttivaMente.Core.OfficeAutomation
{
    public static class WordAutomation
    {
        public static byte[] GetUserDocxBytes(Utente utente, string templatePath)
        {
            using var memoryStream = new MemoryStream();
            using (var document = DocX.Load(templatePath))
            {
                ReplaceSimpleTextInDocument(document, "{nome}", utente.Nome);
                ReplaceSimpleTextInDocument(document, "{cognome}", utente.Cognome);
                // ReplaceSimpleTextInDocument(document, "{tel1}", utente.Tel1);
                // ReplaceSimpleTextInDocument(document, "{tel2}", utente.Tel2);
                ReplaceSimpleTextInDocument(document, "{email}", utente.Email);
                ReplaceSimpleTextInDocument(document, "{ruolo}", utente.Ruolo!.Nome);
                document.SaveAs(memoryStream);
            }
            return memoryStream.ToArray();
        }

        private static void ReplaceSimpleTextInDocument(DocX document, string oldText, string? newText)
        {
            var options = new StringReplaceTextOptions
            {
                SearchValue = oldText,
                NewValue = newText ?? string.Empty,
                RegExOptions = RegexOptions.None
                // NewFormatting = new Formatting() { Bold = true, FontColor = System.Drawing.Color.Green }
            };
            document.ReplaceText(options);
        }
    }
}
