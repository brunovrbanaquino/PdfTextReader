using iText.Kernel.Pdf;
using System;
using System.IO;

namespace PdfTextReader
{
    class Program2
    {
        static void Main(string[] args)
        {
            var pipeline = new Pipeline();

            var lines = pipeline.GetLines("p44");

            //Testing paragraphs
            var process = new Structure.ProcessStructure2();
            var paragraphs = process.ProcessParagraph(lines);
        }        
    }
}
