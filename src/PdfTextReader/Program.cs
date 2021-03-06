using iText.Kernel.Pdf;
using System;
using System.IO;

namespace PdfTextReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("PDF Text Reader");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Program3.ProcessStats2(@"DO1_2016_01_18", 2);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time was: {elapsedMs}");

            Console.ReadKey();
        }
    }
}
