using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AffärsLager.Controllers;

namespace AffärsLager.Services
{
    /// <summary>
    /// PDFService - Genererar PDF-rapporter och köksbongar
    /// Använder QuestPDF-biblioteket för att skapa professionella PDF-dokument
    /// Genererar 3 typer av PDF:er:
    /// 1. Restaurangchef-rapporter (statistik för en restaurang)
    /// 2. VD-rapporter (koncernöversikt över alla 18 restauranger)
    /// 3. Köksbongar (beställningar för köket)
    /// </summary>
    public class PDFService
    {
        public PDFService()
        {
            // Aktivera QuestPDF licens för utveckling
            // Community license är gratis för icke-kommersiellt bruk
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Hittar Logg-mappen genom att söka uppåt från BaseDirectory
        /// </summary>
        private string FindLoggFolder()
        {
            // Starta från BaseDirectory (vanligtvis bin/Debug/net6.0-windows)
            DirectoryInfo? currentDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            // Sök uppåt max 10 nivåer för att hitta Logg-mappen
            for (int i = 0; i < 10 && currentDir != null; i++)
            {
                string loggPath = Path.Combine(currentDir.FullName, "Logg");
                if (Directory.Exists(loggPath))
                {
                    return loggPath;
                }
                currentDir = currentDir.Parent;
            }

            // Om vi inte hittar Logg-mappen, skapa den i projektroten
            // (gå upp 4 nivåer från bin/Debug/net6.0-windows till PresentationsLager, sen till projektroten)
            currentDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 4 && currentDir != null; i++)
            {
                currentDir = currentDir.Parent;
            }

            string fallbackLoggPath = Path.Combine(currentDir?.FullName ?? AppDomain.CurrentDomain.BaseDirectory, "Logg");
            return fallbackLoggPath;
        }

        /// <summary>
        /// Genererar PDF-rapport för restaurangchef
        /// </summary>
        public string GenerateRestaurangchefRapport(
            string restaurangNamn,
            DateTime startDatum,
            DateTime slutDatum,
            ForsaljningsStatistik forsaljning,
            List<RattStatistik> mestSalda,
            List<RattStatistik> minstSalda,
            List<ServitorStatistik> servitorer,
            BokningsStatistik bokningar)
        {
            string fileName = $"RestaurangRapport_{restaurangNamn}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .AlignCenter()
                        .Text($"Statistikrapport - {restaurangNamn}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(10);

                            // Period
                            x.Item().Text($"Period: {startDatum:yyyy-MM-dd} till {slutDatum:yyyy-MM-dd}").FontSize(12).SemiBold();

                            // Försäljningsöversikt
                            x.Item().PaddingTop(10).Text("FÖRSÄLJNINGSÖVERSIKT").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                            x.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(150);
                                    columns.RelativeColumn();
                                });

                                table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Kategori").Bold();
                                table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Belopp").Bold();

                                table.Cell().Border(1).Padding(5).Text("Total Försäljning");
                                table.Cell().Border(1).Padding(5).Text($"{forsaljning.TotalForsaljning:N0} kr");

                                table.Cell().Border(1).Padding(5).Text("Mat");
                                table.Cell().Border(1).Padding(5).Text($"{forsaljning.MatSumma:N0} kr");

                                table.Cell().Border(1).Padding(5).Text("Alkohol");
                                table.Cell().Border(1).Padding(5).Text($"{forsaljning.AlkoholSumma:N0} kr");

                                table.Cell().Border(1).Padding(5).Text("Antal Transaktioner");
                                table.Cell().Border(1).Padding(5).Text($"{forsaljning.AntalTransaktioner}");
                            });

                            // Mest sålda rätter
                            if (mestSalda.Any())
                            {
                                x.Item().PaddingTop(15).Text("MEST SÅLDA RÄTTER").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                                x.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Rätt").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Antal").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Summa").Bold();

                                    foreach (var ratt in mestSalda.Take(10))
                                    {
                                        table.Cell().Border(1).Padding(5).Text(ratt.Rattnamn);
                                        table.Cell().Border(1).Padding(5).Text($"{ratt.AntalSalda}");
                                        table.Cell().Border(1).Padding(5).Text($"{ratt.TotalForsaljning:N0} kr");
                                    }
                                });
                            }

                            // Servitörstatistik
                            if (servitorer.Any())
                            {
                                x.Item().PaddingTop(15).Text("SERVITÖRSTATISTIK").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                                x.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Namn").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Transaktioner").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Försäljning").Bold();

                                    foreach (var servitor in servitorer)
                                    {
                                        table.Cell().Border(1).Padding(5).Text(servitor.Namn);
                                        table.Cell().Border(1).Padding(5).Text($"{servitor.AntalTransaktioner}");
                                        table.Cell().Border(1).Padding(5).Text($"{servitor.TotalForsaljning:N0} kr");
                                    }
                                });
                            }

                            // Bokningsstatistik
                            x.Item().PaddingTop(15).Text("BOKNINGSSTATISTIK").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                            x.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(150);
                                    columns.RelativeColumn();
                                });

                                table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Kategori").Bold();
                                table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Antal").Bold();

                                table.Cell().Border(1).Padding(5).Text("Antal Bokningar");
                                table.Cell().Border(1).Padding(5).Text($"{bokningar.AntalBokningar}");

                                table.Cell().Border(1).Padding(5).Text("Antal Gäster");
                                table.Cell().Border(1).Padding(5).Text($"{bokningar.AntalGaster}");

                                table.Cell().Border(1).Padding(5).Text("Unika Bord");
                                table.Cell().Border(1).Padding(5).Text($"{bokningar.AntalUnikalaBord}");
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span($"Genererad: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(9);
                            x.Span(" | ");
                            x.Span("RestoNation").FontSize(9).SemiBold();
                        });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }

        /// <summary>
        /// Genererar PDF-rapport för VD
        /// </summary>
        public string GenerateVDRapport(
            DateTime startDatum,
            DateTime slutDatum,
            ForsaljningsStatistik koncernForsaljning,
            List<RegionStatistik> regioner,
            List<RattStatistik> mestSaldaKoncern,
            List<RestaurangStatistik> restauranger)
        {
            string fileName = $"VDRapport_Koncern_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .AlignCenter()
                        .Text("VD-RAPPORT - KONCERNÖVERSIKT")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(10);

                            // Period
                            x.Item().Text($"Period: {startDatum:yyyy-MM-dd} till {slutDatum:yyyy-MM-dd}").FontSize(12).SemiBold();

                            // Koncernöversikt
                            x.Item().PaddingTop(10).Text("KONCERNÖVERSIKT").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                            x.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(150);
                                    columns.RelativeColumn();
                                });

                                table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Kategori").Bold();
                                table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Belopp").Bold();

                                table.Cell().Border(1).Padding(5).Text("Total Försäljning");
                                table.Cell().Border(1).Padding(5).Text($"{koncernForsaljning.TotalForsaljning:N0} kr");

                                table.Cell().Border(1).Padding(5).Text("Mat");
                                table.Cell().Border(1).Padding(5).Text($"{koncernForsaljning.MatSumma:N0} kr");

                                table.Cell().Border(1).Padding(5).Text("Alkohol");
                                table.Cell().Border(1).Padding(5).Text($"{koncernForsaljning.AlkoholSumma:N0} kr");

                                table.Cell().Border(1).Padding(5).Text("Antal Transaktioner");
                                table.Cell().Border(1).Padding(5).Text($"{koncernForsaljning.AntalTransaktioner}");
                            });

                            // Regionstatistik
                            if (regioner.Any())
                            {
                                x.Item().PaddingTop(15).Text("REGIONSTATISTIK").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                                x.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Region").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Försäljning").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Transaktioner").Bold();

                                    foreach (var region in regioner)
                                    {
                                        table.Cell().Border(1).Padding(5).Text(region.RegionNamn);
                                        table.Cell().Border(1).Padding(5).Text($"{region.TotalForsaljning:N0} kr");
                                        table.Cell().Border(1).Padding(5).Text($"{region.AntalTransaktioner}");
                                    }
                                });
                            }

                            // Mest sålda rätter koncern
                            if (mestSaldaKoncern.Any())
                            {
                                x.Item().PaddingTop(15).Text("MEST SÅLDA RÄTTER - KONCERNEN").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                                x.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Rätt").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Antal").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Summa").Bold();

                                    foreach (var ratt in mestSaldaKoncern.Take(10))
                                    {
                                        table.Cell().Border(1).Padding(5).Text(ratt.Rattnamn);
                                        table.Cell().Border(1).Padding(5).Text($"{ratt.AntalSalda}");
                                        table.Cell().Border(1).Padding(5).Text($"{ratt.TotalForsaljning:N0} kr");
                                    }
                                });
                            }

                            // Top 10 Restauranger
                            if (restauranger.Any())
                            {
                                x.Item().PaddingTop(15).Text("TOP 10 RESTAURANGER").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                                x.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Restaurang").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Försäljning").Bold();
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Transaktioner").Bold();

                                    foreach (var restaurang in restauranger.Take(10))
                                    {
                                        table.Cell().Border(1).Padding(5).Text(restaurang.RestaurangNamn);
                                        table.Cell().Border(1).Padding(5).Text($"{restaurang.TotalForsaljning:N0} kr");
                                        table.Cell().Border(1).Padding(5).Text($"{restaurang.AntalTransaktioner}");
                                    }
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span($"Genererad: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(9);
                            x.Span(" | ");
                            x.Span("RestoNation").FontSize(9).SemiBold();
                        });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }

        /// <summary>
        /// Genererar köksbong för en beställning och sparar i Logg-mappen
        /// </summary>
        public string GenereraKoksbong(
            string bordnummer,
            DateTime tid,
            List<BestallningsRadDto> ratter,
            string? specialinformation = null)
        {
            // Hitta projektroten genom att söka uppåt från BaseDirectory
            string loggMapp = FindLoggFolder();

            // Skapa mappen om den inte finns
            Directory.CreateDirectory(loggMapp);

            // Räkna antal befintliga köksbongar för detta bord idag för att få löpnummer
            string today = DateTime.Now.ToString("yyyyMMdd");
            var existingFiles = Directory.GetFiles(loggMapp, $"{today}_Koksbong_{bordnummer}_*.pdf");
            int lopnummer = existingFiles.Length + 1;

            string fileName = $"{today}_Koksbong_{bordnummer}_{lopnummer:D3}.pdf";
            string filePath = Path.Combine(loggMapp, fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5); // Mindre format för köket
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14).FontFamily("Arial"));

                    page.Content()
                        .Column(x =>
                        {
                            x.Spacing(15);

                            // BORD - Stort och tydligt
                            x.Item()
                                .AlignCenter()
                                .Text($"BORD {bordnummer}")
                                .Bold()
                                .FontSize(48)
                                .FontColor(Colors.Black);

                            // TID
                            x.Item()
                                .AlignCenter()
                                .Text($"{tid:HH:mm}")
                                .SemiBold()
                                .FontSize(20);

                            // Separator
                            x.Item()
                                .PaddingVertical(5)
                                .LineHorizontal(2)
                                .LineColor(Colors.Black);

                            // RÄTTER
                            x.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);  // Antal
                                    columns.RelativeColumn();    // Rätt
                                });

                                foreach (var ratt in ratter)
                                {
                                    table.Cell().Padding(5).Text($"{ratt.Antal}x").Bold().FontSize(18);
                                    table.Cell().Padding(5).Text(ratt.Rattnamn).FontSize(18);
                                }
                            });

                            // Specialinformation
                            if (!string.IsNullOrWhiteSpace(specialinformation))
                            {
                                x.Item()
                                    .PaddingTop(10)
                                    .Border(2)
                                    .BorderColor(Colors.Red.Medium)
                                    .Padding(10)
                                    .Text($"⚠️ {specialinformation}")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.Red.Darken1);
                            }

                            // Footer
                            x.Item()
                                .PaddingTop(20)
                                .AlignCenter()
                                .Text($"Beställning: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken1);
                        });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }
    }
}