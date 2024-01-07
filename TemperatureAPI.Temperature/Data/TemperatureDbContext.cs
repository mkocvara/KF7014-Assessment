using Microsoft.EntityFrameworkCore;
using TemperatureAPI.Temperature.Models;

namespace TemperatureAPI.Temperature.Data
{
    public class TemperatureDbContext : DbContext
    {
        public TemperatureDbContext(DbContextOptions options) : base(options) { }
        public DbSet<DataEntry> Entries { get; set; } = null!;
    }
}