using ClientApp.Data;
using static MoreLinq.Extensions.FullGroupJoinExtension;
using static MoreLinq.Extensions.FullJoinExtension;

namespace ClientApp.Data.Repositories
{
    public class AggregateRepository : IReadOnlyRepository<WeatherMeasurement>
    {
        private readonly IReadOnlyRepository<PrecipitationMeasurement> _precipitationRepository;
        private readonly IReadOnlyRepository<TemperatureMeasurement> _temperatureRepository;
        private readonly IReadOnlyRepository<HumidityMeasurement> _humidityRepository;

        public AggregateRepository(IReadOnlyRepository<PrecipitationMeasurement> precipitationRepository,
                                   IReadOnlyRepository<TemperatureMeasurement> temperatureRepository,
                                   IReadOnlyRepository<HumidityMeasurement> humidityRepository)
        {
            _precipitationRepository = precipitationRepository;
            _temperatureRepository = temperatureRepository;
            _humidityRepository = humidityRepository;
        }

        public async Task<IEnumerable<WeatherMeasurement>> GetAll()
        {
            try
            {
                List<PrecipitationMeasurement> precipMeasurements = (await _precipitationRepository.GetAll()).ToList();
                List<TemperatureMeasurement> tempMeasurements = (await _temperatureRepository.GetAll()).ToList();
                List<HumidityMeasurement> humidityMeasurements = (await _humidityRepository.GetAll()).ToList();

               
                // ignore measurements without a date or location, as they cannot be aggregated
                IEnumerable<PrecipitationMeasurement> pmHasDateAndLoc = precipMeasurements.Where(pm => pm.DateTime.HasValue && pm.Location is not null);
                IEnumerable<TemperatureMeasurement> tmHasDateAndLoc = tempMeasurements.Where(tm => tm.DateTime.HasValue && tm.Location is not null);
                IEnumerable<HumidityMeasurement> hmHasDateAndLoc = humidityMeasurements.Where(hm => hm.Timestamp.HasValue && hm.Location is not null);

#pragma warning disable CS8629 // DateTime cannot be null here.
                // where measurements share the same location and date, keep only the most recent
                IEnumerable<IGrouping<string?, PrecipitationMeasurement>> pmByLocation = pmHasDateAndLoc.GroupBy(pm => pm.Location);
                List<PrecipitationMeasurement> precipMeasurementsFiltered = new();
                foreach (IGrouping<string?, PrecipitationMeasurement> group in pmByLocation)
                {
                    IEnumerable<PrecipitationMeasurement> onePerDay = group.GroupBy(pm => pm.DateTime.Value.Date)
                                                                           .Select(g => g.OrderBy(pm => pm.DateTime)
                                                                                         .Last());
                    precipMeasurementsFiltered.AddRange(onePerDay);
                }

                IEnumerable<IGrouping<string?, TemperatureMeasurement>> tmByLocation = tmHasDateAndLoc.GroupBy(tm => tm.Location);
                List<TemperatureMeasurement> tempMeasurementsFiltered = new();
                foreach (IGrouping<string?, TemperatureMeasurement> group in tmByLocation)
                {
                    IEnumerable<TemperatureMeasurement> onePerDay = group.GroupBy(tm => tm.DateTime.Value.Date)
                                                                           .Select(g => g.OrderBy(tm => tm.DateTime)
                                                                                         .Last());
                    tempMeasurementsFiltered.AddRange(group);
                }

                IEnumerable<IGrouping<string?, HumidityMeasurement>> hmByLocation = hmHasDateAndLoc.GroupBy(hm => hm.Location);
                List<HumidityMeasurement> humidityMeasurementsFiltered = new();
                foreach (IGrouping<string?, HumidityMeasurement> group in hmByLocation)
                {
                    IEnumerable<HumidityMeasurement> onePerDay = group.GroupBy(hm => hm.Timestamp.Value.Date)
                                                                           .Select(g => g.OrderBy(hm => hm.Timestamp)
                                                                                         .Last());
                    humidityMeasurementsFiltered.AddRange(group);
                }

                // full outer join measurements on date and location to create aggregate measurements
                IEnumerable<WeatherMeasurement> measurements = precipMeasurementsFiltered.FullJoin(
                    tempMeasurementsFiltered,
                    pm => new { loc = pm.Location, date = pm.DateTime.Value.Date },
                    tm => new { loc = tm.Location, date = tm.DateTime.Value.Date },
                    (pm) => new WeatherMeasurement().CopyDataFromPrecipitationMeasurement(pm),
                    (tm) => new WeatherMeasurement().CopyDataFromTemperatureMeasurement(tm),
                    (pm, tm) => new WeatherMeasurement().CopyDataFromPrecipitationMeasurement(pm)
                                                        .CopyDataFromTemperatureMeasurement(tm)
                );

                measurements = measurements.FullJoin(
                    humidityMeasurementsFiltered,
                    wm => new { loc = wm.Location, date = wm.DateTime.Value.Date },
                    hm => new { loc = hm.Location, date = hm.Timestamp.Value.Date },
                    (wm) => wm,
                    (hm) => new WeatherMeasurement().CopyDataFromHumidityMeasurement(hm),
                    (wm, hm) => wm.CopyDataFromHumidityMeasurement(hm)
                ); 
#pragma warning restore CS8629

                if (measurements == null)
                    return new List<WeatherMeasurement>();
                else
                    return measurements;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<WeatherMeasurement>();
            }
        }

        public async Task<IEnumerable<WeatherMeasurement>> GetAllByLocation(string location)
        {
            try
            {
                List<PrecipitationMeasurement> precipMeasurements = (await _precipitationRepository.GetAllByLocation(location)).ToList();
                List<TemperatureMeasurement> tempMeasurements = (await _temperatureRepository.GetAllByLocation(location)).ToList();
                List<HumidityMeasurement> humidityMeasurements = (await _humidityRepository.GetAllByLocation(location)).ToList();

                // ignore measurements without a date, as they cannot be aggregated
                IEnumerable<PrecipitationMeasurement> pmHasDate = precipMeasurements.Where(pm => pm.DateTime.HasValue);
                IEnumerable<TemperatureMeasurement> tmHasDate = tempMeasurements.Where(tm => tm.DateTime.HasValue);
                IEnumerable<HumidityMeasurement> hmHasDate = humidityMeasurements.Where(hm => hm.Timestamp.HasValue);

#pragma warning disable CS8629 // DateTime cannot be null here.
                // where measurements share the same date, keep only the most recent
                pmHasDate = pmHasDate.GroupBy(pm => pm.DateTime.Value.Date)
                                                 .Select(g => g.OrderBy(pm => pm.DateTime)
                                                               .Last());
                tmHasDate = tmHasDate.GroupBy(tm => tm.DateTime.Value.Date)
                                                 .Select(g => g.OrderBy(tm => tm.DateTime)
                                                               .Last());
                hmHasDate = hmHasDate.GroupBy(hm => hm.Timestamp.Value.Date)
                                                 .Select(g => g.OrderBy(hm => hm.Timestamp)
                                                               .Last());

                // full outer join measurements on date to create aggregate measurements
                IEnumerable<WeatherMeasurement> measurements = pmHasDate.FullJoin(
                    tmHasDate,
                    pm => new { date = pm.DateTime.Value.Date },
                    tm => new { date = tm.DateTime.Value.Date },
                    (pm) => new WeatherMeasurement().CopyDataFromPrecipitationMeasurement(pm),
                    (tm) => new WeatherMeasurement().CopyDataFromTemperatureMeasurement(tm),
                    (pm, tm) => new WeatherMeasurement().CopyDataFromPrecipitationMeasurement(pm)
                                                        .CopyDataFromTemperatureMeasurement(tm)
                );

                measurements = measurements.FullJoin(
                    hmHasDate,
                    wm => new { date = wm.DateTime.Value.Date },
                    hm => new { date = hm.Timestamp.Value.Date },
                    (wm) => wm,
                    (hm) => new WeatherMeasurement().CopyDataFromHumidityMeasurement(hm),
                    (wm, hm) => wm.CopyDataFromHumidityMeasurement(hm)
                );
#pragma warning restore CS8629

                if (measurements == null)
                    return new List<WeatherMeasurement>();
                else
                    return measurements;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<WeatherMeasurement>();
            }
        }

        public Task<WeatherMeasurement?> GetById(int id)
        {
            // Can't get an aggregate measurement from an ID
            return Task.FromResult<WeatherMeasurement?>(null);
        }

        public async Task<WeatherMeasurement?> GetLatestInLocation(string location)
        {
            try
            {
                PrecipitationMeasurement precipMeasurement = await _precipitationRepository.GetLatestInLocation(location) ?? new();
                TemperatureMeasurement tempMeasurement = await _temperatureRepository.GetLatestInLocation(location) ?? new();
                HumidityMeasurement humidityMeasurement = await _humidityRepository.GetLatestInLocation(location)?? new();

                DateTime? latestDate = null;
                if (precipMeasurement.DateTime.HasValue)
                    latestDate = precipMeasurement.DateTime.Value.Date;
                if (tempMeasurement.DateTime.HasValue)
                    latestDate = latestDate.HasValue ? new List<DateTime> { latestDate.Value.Date, tempMeasurement.DateTime.Value.Date }.Max() : tempMeasurement.DateTime.Value.Date;
                if (humidityMeasurement.Timestamp.HasValue)
                    latestDate = latestDate.HasValue ? new List<DateTime> { latestDate.Value.Date, humidityMeasurement.Timestamp.Value.Date }.Max() : humidityMeasurement.Timestamp.Value.Date;

                WeatherMeasurement wm = new();
                if (precipMeasurement.DateTime.HasValue && precipMeasurement.DateTime.Value.Date == latestDate)
                    wm.CopyDataFromPrecipitationMeasurement(precipMeasurement);
                if (tempMeasurement.DateTime.HasValue && tempMeasurement.DateTime.Value.Date == latestDate)
                    wm.CopyDataFromTemperatureMeasurement(tempMeasurement);
                if (humidityMeasurement.Timestamp.HasValue && humidityMeasurement.Timestamp.Value.Date == latestDate)
                    wm.CopyDataFromHumidityMeasurement(humidityMeasurement);

                return wm;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<WeatherMeasurement>> GetLatestInEveryLocation()
        {
            try
            {
                IEnumerable<WeatherMeasurement> measurements = await GetAll();

                // where measurements share the same LOCATION, keep only the most recent
                measurements = measurements.GroupBy(pm => pm.Location)
                                            .Select(g => g.OrderBy(pm => pm.DateTime)
                                                        .Last());

                if (measurements == null)
                    return new List<WeatherMeasurement>();
                else
                    return measurements;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<WeatherMeasurement>();
            }
        }
    }
}
