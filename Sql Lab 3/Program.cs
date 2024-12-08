using System;
using System.IO;
using System.Linq;
using Core.Models;
using Core.Services;
using DataAccess;
using Spectre.Console;

namespace Sql_Lab_3
{
    class Program
    {
        static void Main(string[] args)
        {
            using var context = new AppDbContext();
            context.Database.EnsureCreated();
            DataSeeder.SeedData(context); // Initiera data om databasen är tom

            bool running = true;

            while (running)
            {
                Console.Clear();
                AnsiConsole.Markup("[bold yellow]Välkommen till Jakob/Abdirahim/Abdirahmans Dataväder Analys![/]\n");
                AnsiConsole.MarkupLine("[dim]Använd piltangenterna för att navigera och tryck [bold green]Enter[/] för att välja ett alternativ.[/]\n");

                var options = new[]
                {
                    "Visa alla data",
                    "Visa medeltemperatur",
                    "Visa sortering efter temperatur",
                    "Visa sortering efter luftfuktighet",
                    "Visa sortering efter mögelrisk",
                    "Visa meteorologisk höst",
                    "Visa meteorologisk vinter",
                    "Läs in data från CSV-fil",
                    "Visa skillnader mellan inomhus- och utomhustemperatur",
                    "Visa balkongdörrens öppettider",
                    "Visa medeltemperatur för valt datum utomhus",
                    "Sortera utomhus medeltemperatur från varmast till kallast",
                    "Sortera utomhus medelluftfuktighet från torrast till fuktigast",
                    "[red]Avsluta[/]"
                };

                var selectedOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold blue]Välj ett alternativ:[/]")
                        .PageSize(10)
                        .AddChoices(options));

                Console.Clear();

                try
                {
                    switch (selectedOption)
                    {
                        case "Visa alla data":
                            ShowAllData(context);
                            break;
                        case "Visa medeltemperatur":
                            ShowAverageTemperature(context);
                            break;
                        case "Visa sortering efter temperatur":
                            ShowSortedByTemperature(context);
                            break;
                        case "Visa sortering efter luftfuktighet":
                            ShowSortedByHumidity(context);
                            break;
                        case "Visa sortering efter mögelrisk":
                            ShowSortedByMoldRisk(context);
                            break;
                        case "Visa meteorologisk höst":
                            ShowMeteorologicalAutumn(context);
                            break;
                        case "Visa meteorologisk vinter":
                            ShowMeteorologicalWinter(context);
                            break;
                        case "Läs in data från CSV-fil":
                            ImportFromCsv(context);
                            break;
                        case "Visa skillnader mellan inomhus- och utomhustemperatur":
                            ShowTemperatureDifferences(context);
                            break;
                        case "Visa balkongdörrens öppettider":
                            ShowBalconyDoorOpenings(context);
                            break;
                        case "Visa medeltemperatur för valt datum utomhus":
                            ShowOutdoorAverageTemperatureForDate(context);
                            break;
                        case "Sortera utomhus medeltemperatur från varmast till kallast":
                            ShowOutdoorTemperatureSorted(context);
                            break;
                        case "Sortera utomhus medelluftfuktighet från torrast till fuktigast":
                            ShowOutdoorHumiditySorted(context);
                            break;
                        case "[red]Avsluta[/]":
                            running = false;
                            break;
                        default:
                            AnsiConsole.MarkupLine("[red]Ogiltigt val. Försök igen.[/]");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[bold red]Ett fel inträffade:[/] {ex.Message}");
                }
            }
        }

        static void ShowAllData(AppDbContext context)
        {
            var records = context.TempHumidityRecords.ToList();
            Console.WriteLine("\nAlla data:");
            foreach (var record in records)
            {
                Console.WriteLine($"{record.Date:yyyy-MM-dd}, {record.Temperature}°C, {record.Humidity}%, {(record.IsIndoor ? "Inomhus" : "Utomhus")}");
            }
            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowAverageTemperature(AppDbContext context)
        {
            var records = context.TempHumidityRecords.ToList();
            var avgTemp = CalculationService.CalculateAverageTemperature(records);
            Console.WriteLine($"\nMedeltemperatur: {avgTemp:F2}°C");
            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowSortedByTemperature(AppDbContext context)
        {
            var records = CalculationService.SortByTemperature(context.TempHumidityRecords.ToList());
            Console.WriteLine("\nSorterat efter temperatur (högst till lägst):");
            foreach (var record in records)
            {
                Console.WriteLine($"{record.Date:yyyy-MM-dd}, {record.Temperature}°C");
            }
            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowSortedByHumidity(AppDbContext context)
        {
            var records = CalculationService.SortByHumidity(context.TempHumidityRecords.ToList());
            Console.WriteLine("\nSorterat efter luftfuktighet (högst till lägst):");
            foreach (var record in records)
            {
                Console.WriteLine($"{record.Date:yyyy-MM-dd}, {record.Humidity}%");
            }
            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowSortedByMoldRisk(AppDbContext context)
        {
            var records = CalculationService.SortByMoldRisk(context.TempHumidityRecords.ToList());
            Console.WriteLine("\nSorterat efter mögelrisk (högst till lägst):");
            foreach (var (record, moldRisk) in records)
            {
                Console.WriteLine($"{record.Date:yyyy-MM-dd}, {record.Temperature}°C, {record.Humidity}%, Mögelrisk: {moldRisk:F2}");
            }
            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowMeteorologicalAutumn(AppDbContext context)
        {
            var records = context.TempHumidityRecords.ToList();
            var autumnDate = CalculationService.GetMeteorologicalAutumn(records);

            if (autumnDate.HasValue)
            {
                Console.WriteLine($"Meteorologisk höst börjar {autumnDate.Value:yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine("Ingen meteorologisk höst identifierades.");
            }

            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowMeteorologicalWinter(AppDbContext context)
        {
            var records = context.TempHumidityRecords.ToList();
            var winterDate = CalculationService.GetMeteorologicalWinter(records);

            if (winterDate.HasValue)
            {
                Console.WriteLine($"Meteorologisk vinter börjar {winterDate.Value:yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine("Ingen meteorologisk vinter identifierades.");
            }

            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowTemperatureDifferences(AppDbContext context)
        {
            var records = context.TempHumidityRecords.ToList();
            var (maxDiff, minDiff, maxIndoor, maxOutdoor, minIndoor, minOutdoor) =
                CalculationService.SortIndoorOutdoorDifferences(records);

            Console.WriteLine("\nStörsta temperaturskillnad:");
            Console.WriteLine($"Inomhus: {maxIndoor.Temperature}°C, Utomhus: {maxOutdoor.Temperature}°C, Skillnad: {maxDiff:F2}°C");
            Console.WriteLine("\nMinsta temperaturskillnad:");
            Console.WriteLine($"Inomhus: {minIndoor.Temperature}°C, Utomhus: {minOutdoor.Temperature}°C, Skillnad: {minDiff:F2}°C");

            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowBalconyDoorOpenings(AppDbContext context)
        {
            var records = context.TempHumidityRecords.ToList();
            var balconyOpenings = CalculationService.CalculateDailyBalconyOpenTimes(records);

            Console.WriteLine("\nBalkongdörrens öppettider per dag:");
            foreach (var (date, totalOpenTime) in balconyOpenings)
            {
                Console.WriteLine($"{date:yyyy-MM-dd}: {totalOpenTime.TotalMinutes} minuter");
            }
            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowOutdoorAverageTemperatureForDate(AppDbContext context)
        {
            Console.Write("Ange datum (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime selectedDate))
            {
                var records = context.TempHumidityRecords
                    .Where(r => r.Date.Date == selectedDate && !r.IsIndoor)
                    .ToList();

                if (records.Any())
                {
                    var avgTemp = records.Average(r => r.Temperature);
                    Console.WriteLine($"Medeltemperatur utomhus för {selectedDate:yyyy-MM-dd}: {avgTemp:F2}°C");
                }
                else
                {
                    Console.WriteLine("Inga utomhusposter hittades för detta datum.");
                }
            }
            else
            {
                Console.WriteLine("Ogiltigt datumformat.");
            }

            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowOutdoorTemperatureSorted(AppDbContext context)
        {
            var records = context.TempHumidityRecords
                .Where(r => !r.IsIndoor)
                .GroupBy(r => r.Date.Date)
                .Select(g => new { Date = g.Key, AvgTemperature = g.Average(r => r.Temperature) })
                .OrderByDescending(g => g.AvgTemperature)
                .ToList();

            Console.WriteLine("\nSortering av utomhus medeltemperatur (varmast till kallast):");
            foreach (var record in records)
            {
                Console.WriteLine($"{record.Date:yyyy-MM-dd}: {record.AvgTemperature:F2}°C");
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ShowOutdoorHumiditySorted(AppDbContext context)
        {
            var records = context.TempHumidityRecords
                .Where(r => !r.IsIndoor)
                .GroupBy(r => r.Date.Date)
                .Select(g => new { Date = g.Key, AvgHumidity = g.Average(r => r.Humidity) })
                .OrderBy(g => g.AvgHumidity)
                .ToList();

            Console.WriteLine("\nSortering av utomhus medelluftfuktighet (torrast till fuktigast):");
            foreach (var record in records)
            {
                Console.WriteLine($"{record.Date:yyyy-MM-dd}: {record.AvgHumidity:F2}%");
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        static void ImportFromCsv(AppDbContext context)
        {
            // Ange den fullständiga sökvägen till CSV-filen
            //string filePath = @"C:\Users\jakob\source\repos\Sql Lab 3\Sql Lab 3\weather_data.csv";
            //string filePath = @"weather_data.csv";
            string filePath = @"TempFuktData.csv";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Filen hittades inte.");
                Console.ReadKey();
                return;
            }

            try
            {
                var recordsWithMoldRisk = CsvImporter.Import(filePath);  // Läs in CSV-filen
                foreach (var (record, moldRisk) in recordsWithMoldRisk)
                {
                    if (!context.TempHumidityRecords.Any(r =>
                        r.Date == record.Date &&
                        r.Temperature == record.Temperature &&
                        r.Humidity == record.Humidity))
                    {
                        context.TempHumidityRecords.Add(record);  // Lägg till datan i databasen
                        //Console.WriteLine($"Lagt till: {record.Date:yyyy-MM-dd}, {record.Temperature}°C, {record.Humidity}%, Mögelrisk: {moldRisk:F2}");
                    }
                }
                context.SaveChanges();  // Spara ändringarna till databasen
                Console.WriteLine("Import från CSV lyckades.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade vid inläsning: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
