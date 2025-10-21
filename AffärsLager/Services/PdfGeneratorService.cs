using AffärsLager.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;

namespace AffärsLager.Services
{
    public class PdfGeneratorService
    {
        private readonly string _pdfMapp;

        public PdfGeneratorService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _pdfMapp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PDFRapporter");
            if (!Directory.Exists(_pdfMapp))
            {
                Directory.CreateDirectory(_pdfMapp);
            }
        }

        public string GeneraStatistikRapport(ForsaljningsSummaryDto data)
        {
            var filnamn = $"Statistik_{data.RestaurangNamn}_{DateTime.Now:yyyy-MM-dd_HHmmss}.pdf";
            var filSokväg = Path.Combine(_pdfMapp, filnamn);
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text($"Försäljningsrapport - {data.RestaurangNamn}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        // Period
                        column.Item().Text($"Period: {data.FranDatum:yyyy-MM-dd} till {data.TillDatum:yyyy-MM-dd}");

                        // Sammanfattning
                        column.Item().PaddingVertical(10).Text("Sammanfattning").FontSize(16).SemiBold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Total försäljning:");
                            table.Cell().Text($"{data.TotalForsaljning:C}").AlignRight();

                            table.Cell().Text("Antal beställningar:");
                            table.Cell().Text($"{data.TotaltAntalBestallningar}").AlignRight();

                            table.Cell().Text("Antal bokningar:");
                            table.Cell().Text($"{data.TotaltAntalBokningar}").AlignRight();

                            table.Cell().Text("Antal gäster:");
                            table.Cell().Text($"{data.TotaltAntalGaster}").AlignRight();

                            table.Cell().Text("Mat:");
                            table.Cell().Text($"{data.MatForsaljning:C}").AlignRight();

                            table.Cell().Text("Dryck:");
                            table.Cell().Text($"{data.DryckForsaljning:C}").AlignRight();
                        });

                        // Toppsäljare
                        column.Item().PaddingVertical(10).Text("Mest sålda rätter").FontSize(14).SemiBold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Rätt").SemiBold();
                                header.Cell().Text("Antal").SemiBold();
                                header.Cell().Text("Summa").SemiBold();
                            });

                            foreach (var rätt in data.MestSaldaRatter)
                            {
                                table.Cell().Text(rätt.Rattnamn);
                                table.Cell().Text($"{rätt.AntalSalda}");
                                table.Cell().Text($"{rätt.TotalForsaljning:C}");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Sida ");
                        x.CurrentPageNumber();
                        x.Span(" av ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf(filSokväg);

            return filSokväg;
        }
    }
}