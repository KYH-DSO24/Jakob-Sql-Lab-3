using Core.Models;
using System;
using System.Linq;

namespace DataAccess
{
    public static class DataSeeder
    {
        /// <summary>
        /// Seedar testdata i databasen om den är tom.
        /// </summary>
        /// <param name="context">Databaskontexten som används.</param>
        public static void SeedData(AppDbContext context)
        {
            // Kontrollera om data redan finns i databasen
            if (!context.TempHumidityRecords.Any())
            {
                context.TempHumidityRecords.AddRange(new[]
                {
                    // Inomhusdata
                    new TempHumidityRecord { Date = DateTime.Now.AddDays(-10), Temperature = 22.5, Humidity = 40, IsIndoor = true },
                    new TempHumidityRecord { Date = DateTime.Now.AddDays(-9), Temperature = 21.0, Humidity = 45, IsIndoor = true },
                    new TempHumidityRecord { Date = DateTime.Now.AddDays(-8), Temperature = 20.5, Humidity = 50, IsIndoor = true },
                    new TempHumidityRecord { Date = DateTime.Now.AddDays(-7), Temperature = 19.0, Humidity = 55, IsIndoor = true },
                    
                    // Utomhusdata - höst
                    new TempHumidityRecord { Date = DateTime.Now.AddDays(-6), Temperature = 10.0, Humidity = 75, IsIndoor = false },
                    new TempHumidityRecord { Date = DateTime.Now.AddDays(-5), Temperature = 8.0, Humidity = 80, IsIndoor = false },
                    new TempHumidityRecord { Date = DateTime.Now.AddDays(-4), Temperature = 12.0, Humidity = 70, IsIndoor = false },

                    // Utomhusdata - vinter
                    new TempHumidityRecord { Date = new DateTime(2023, 12, 15), Temperature = -2.0, Humidity = 85, IsIndoor = false },
                    new TempHumidityRecord { Date = new DateTime(2023, 12, 16), Temperature = -5.0, Humidity = 90, IsIndoor = false },

                    // Blandad data - vår
                    new TempHumidityRecord { Date = new DateTime(2023, 3, 15), Temperature = 5.0, Humidity = 60, IsIndoor = false },
                    new TempHumidityRecord { Date = new DateTime(2023, 3, 16), Temperature = 7.0, Humidity = 55, IsIndoor = false },

                    // Inomhusdata under vintertid
                    new TempHumidityRecord { Date = new DateTime(2023, 12, 15), Temperature = 22.0, Humidity = 35, IsIndoor = true },
                    new TempHumidityRecord { Date = new DateTime(2023, 12, 16), Temperature = 21.5, Humidity = 40, IsIndoor = true }
                });

                // Spara ändringar i databasen
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Rensar all data från databasen (använd vid behov för tester).
        /// </summary>
        /// <param name="context">Databaskontexten som används.</param>
        public static void ClearData(AppDbContext context)
        {
            context.TempHumidityRecords.RemoveRange(context.TempHumidityRecords);
            context.SaveChanges();
        }
    }
}
