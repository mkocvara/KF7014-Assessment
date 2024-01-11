using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemperatureAPI.Temperature.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace TemperatureAPI.Temperature.Models
{
    public class TemperatureRepository : ITemperatureRepository
    {
        private readonly TemperatureDbContext _context;
        public TemperatureRepository(TemperatureDbContext context)
        {
            _context = context;
        }
        public async Task<List<TemperatureModel>> GetHistoricalTemperatureEntries()
        {
            var entries = await _context.Entries.ToListAsync();

            var historicalEntries = entries
                .Select(entry => new TemperatureModel
                {
                    Id = entry.Id,
                    Temperature = entry.Temperature,
                    Date = DateTime.ParseExact(entry.Date, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
                    Sensor = entry.SensorDescription
                })
                .ToList();

            return historicalEntries;
        }

        public async Task<List<TemperatureModel>> GetLatestTemperatureEntries()
        {
            var entries = await _context.Entries
                .GroupBy(entry => entry.SensorId)
                .Select(group => group
                    .OrderByDescending(entry => entry.Date)
                    .FirstOrDefault())
                .ToListAsync();

            var latestEntries = entries.Select(entry => new TemperatureModel
            {
                Id = entry.Id,
                Temperature = entry.Temperature,
                Date = DateTime.ParseExact(entry.Date, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
                Sensor = entry.SensorDescription
            }).ToList();

            return latestEntries;
        }



        public async Task<TemperatureModel?> GetTemperatureEntry(int id)
        {
            DataEntry? entry = await _context.Entries.FindAsync(id);

            if (entry is null){
                return null;
            }

            return new TemperatureModel()
            {
                Id = entry.Id,
                Temperature = entry.Temperature,
                Date = DateTime.TryParseExact(entry.Date, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
                        ? parsedDate
                        : DateTime.MinValue,
                Sensor = entry.SensorDescription
            };
        }

        public async Task<DataEntry> AddTemperatureEntry(DataEntry entry)
        {
            DataEntry createdEntry = (await _context.Entries.AddAsync(
                new DataEntry()
                    {
                        Id = entry.Id,
                        Temperature = entry.Temperature,
                        Date = entry.Date,
                        SensorDescription = entry.SensorDescription,
                        SensorId = entry.SensorId
                    })).Entity;
            await _context.SaveChangesAsync();
            return createdEntry;
        }

        public async Task<DataEntry> UpdateTemperatureEntry(int id, DataEntry newEntry)
        {
            DataEntry? entry = await _context.Entries.FindAsync(id);

            entry!.CopyFrom(newEntry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task DeleteTemperatureEntry(int id)
        {
            DataEntry? entry = await _context.Entries.FindAsync(id);
            _context.Entries.Remove(entry!);
            await _context.SaveChangesAsync();
        }
    }
}
