using AffärsLager.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AffärsLager.Services
{
    /// <summary>
    /// Service för att generera bokföringsfiler (CSV/JSON)
    /// </summary>
    public class BokforingService
    {
        private readonly StatistikService _statistikService;
        private readonly string _bokforingMapp;

        public BokforingService()
        {
            _statistikService = new StatistikService();

            // Skapa bokföringsmapp om den inte finns
            _bokforingMapp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bokforing");
            if (!Directory.Exists(_bokforingMapp))
            {
                Directory.CreateDirectory(_bokforingMapp);
            }
        }

        /// <summary>
        /// Generera CSV-fil för en restaurang och datum
        /// </summary>
        public string GenereraCsvFil(int restaurangId, DateTime datum)
        {
            var bokforing = _statistikService.GenereraBokforing(restaurangId, datum);
            var filnamn = $"Bokforing_{bokforing.RestaurangNamn.Replace(" ", "_")}_{datum:yyyy-MM-dd}.csv";
            var filSokväg = Path.Combine(_bokforingMapp, filnamn);

            var csv = new StringBuilder();

            // Header
            csv.AppendLine("Datum;Restaurang;Restaurang-ID;Dagssumma;Antal Transaktioner;Dricks;Kontant;Kort;Lojalitetspoäng");

            // Data
            csv.AppendLine($"{bokforing.Datum:yyyy-MM-dd};" +
                          $"{bokforing.RestaurangNamn};" +
                          $"{bokforing.RestaurangID};" +
                          $"{bokforing.Dagssumma.ToString("F2", CultureInfo.InvariantCulture)};" +
                          $"{bokforing.AntalTransaktioner};" +
                          $"{bokforing.Dricks.ToString("F2", CultureInfo.InvariantCulture)};" +
                          $"{bokforing.KontantBetalningar.ToString("F2", CultureInfo.InvariantCulture)};" +
                          $"{bokforing.KortBetalningar.ToString("F2", CultureInfo.InvariantCulture)};" +
                          $"{bokforing.LojalitetsPoangAnvanda.ToString("F2", CultureInfo.InvariantCulture)}");

            File.WriteAllText(filSokväg, csv.ToString(), Encoding.UTF8);
            return filSokväg;
        }

        /// <summary>
        /// Generera JSON-fil för en restaurang och datum
        /// </summary>
        public string GenereraJsonFil(int restaurangId, DateTime datum)
        {
            var bokforing = _statistikService.GenereraBokforing(restaurangId, datum);
            var filnamn = $"Bokforing_{bokforing.RestaurangNamn.Replace(" ", "_")}_{datum:yyyy-MM-dd}.json";
            var filSokväg = Path.Combine(_bokforingMapp, filnamn);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(bokforing, options);
            File.WriteAllText(filSokväg, json, Encoding.UTF8);

            return filSokväg;
        }

        /// <summary>
        /// Generera samlad bokföring för alla restauranger
        /// </summary>
        public string GenereraSamladBokforingCsv(DateTime datum)
        {
            var filnamn = $"Bokforing_ALLA_Restauranger_{datum:yyyy-MM-dd}.csv";
            var filSokväg = Path.Combine(_bokforingMapp, filnamn);

            var csv = new StringBuilder();
            csv.AppendLine("Restaurang-ID;Restaurangnamn;Datum;Dagssumma;Antal Transaktioner");

            decimal koncernTotal = 0;

            for (int restId = 1; restId <= 18; restId++)
            {
                try
                {
                    var bokforing = _statistikService.GenereraBokforing(restId, datum);
                    csv.AppendLine($"{bokforing.RestaurangID};" +
                                  $"{bokforing.RestaurangNamn};" +
                                  $"{datum:yyyy-MM-dd};" +
                                  $"{bokforing.Dagssumma.ToString("F2", CultureInfo.InvariantCulture)};" +
                                  $"{bokforing.AntalTransaktioner}");
                    koncernTotal += bokforing.Dagssumma;
                }
                catch { }
            }

            csv.AppendLine($"KONCERN TOTALT;;{datum:yyyy-MM-dd};{koncernTotal.ToString("F2", CultureInfo.InvariantCulture)};");

            File.WriteAllText(filSokväg, csv.ToString(), Encoding.UTF8);
            return filSokväg;
        }
    }
}