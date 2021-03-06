﻿using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.ExecutionStats;
using PdfTextReader.TextStructures;
using PdfTextReader.Execution;
using PdfTextReader.PDFText;
using System.Drawing;
using PdfTextReader.PDFCore;

namespace PdfTextReader
{
    class Program4
    {
        public static void ProcessStats(string basename)
        {
            //PdfWriteText.Test();
            //return;
            Console.WriteLine();
            Console.WriteLine("Program4 - Processing with hierachy");
            Console.WriteLine();

            // Extract(1);

            Examples.FollowText(basename);
            Examples.ShowHeaderFooter(basename);
            ExamplesWork.PrintAllSteps(basename, "bin", "bin");

            PdfReaderException.ContinueOnException();

            var conteudos = GetTextLinesWithPipelineBlockset(basename, out Execution.Pipeline pipeline)
                                .Log<AnalyzeLines>($"bin/{basename}-lines.txt")
                            .ConvertText<CreateStructures, TextStructure>()
                                .Log<AnalyzeStructuresCentral>($"bin/{basename}-central.txt")
                            //.PrintAnalytics($"bin/{basename}-print-analytics.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"bin/{basename}-tree.txt")
                                .Log<AnalyzeSegmentStats>($"bin/{basename}-segments-stats.txt")
                                .Log<AnalyzeSegments2>($"bin/{basename}-segments.csv")
                            .ConvertText<CreateTreeSegments, TextSegment>()
                            .ConvertText<TransformConteudo, Conteudo>()
                            .ToList();

            var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();

            //Create XML
            var createArticle = new TransformArtigo();
            var artigos = createArticle.Create(conteudos);
            createArticle.CreateXML(artigos, basename);

            var validator = new ProgramValidatorXML();
            validator.ValidateArticle("bin");
        }


        static PipelineText<TextLine> GetTextLinesWithPipelineBlockset(string basename, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-test-output.pdf")
                    .AllPagesExcept<CreateTextLines>(new int[] { }, page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()
                              .ParsePdf<PreProcessImages>()
                                      .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                                  .ParseBlock<RemoveOverlapedImages>()
                              .ParsePdf<ProcessPdfText>()
                                  .Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                  .ParseBlock<RemoveSmallFonts>()
                                  //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<MergeTableText>()
                                  //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                                  .ParseBlock<HighlightTextTable>()
                                  .ParseBlock<RemoveTableText>()
                                  .ParseBlock<GroupLines>()
                                      .Show(Color.Yellow)
                                      .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<RemoveHeaderImage>()

                                  .ParseBlock<FindInitialBlocksetWithRewind>()
                                      .Show(Color.Gray)
                                  .ParseBlock<BreakColumnsLight>()
                                      //.ParseBlock<BreakColumns>()
                                      .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                      .ParseBlock<RemoveFooter>()
                                  .ParseBlock<AddTableSpace>()
                                  .ParseBlock<AddImageSpace>()
                                  .ParseBlock<BreakInlineElements>()
                                  .ParseBlock<ResizeBlocksets>()
                                      .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Red))
                                  .ParseBlock<OrderBlocksets>()
                                  .Show(Color.Orange)
                                  .ShowLine(Color.Black)
                    );

            return result;
        }
    }
}
