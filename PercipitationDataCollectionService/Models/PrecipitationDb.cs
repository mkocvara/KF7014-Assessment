using Microsoft.EntityFrameworkCore;

namespace PrecipitationService.Db
{
    public class PrecipitationDb : DbContext
    {
        public PrecipitationDb(DbContextOptions<PrecipitationDb> options) : base(options) { }

        public DbSet<PrecipitationMeasurement> Measurements => Set<PrecipitationMeasurement>();
    }
}
