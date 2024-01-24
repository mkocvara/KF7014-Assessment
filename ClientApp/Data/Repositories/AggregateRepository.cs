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
                Console.WriteLine(tempMeasurements.Count());

                // DEBUG TEMP
                tempMeasurements.Add(new TemperatureMeasurement("Newcastle upon Tyne", DateTime.Today - TimeSpan.FromDays(1), 10));
                tempMeasurements.Add(new TemperatureMeasurement("Leeds", DateTime.Today - TimeSpan.FromDays(2), 154.4f));
                tempMeasurements.Add(new TemperatureMeasurement("Nottingham", DateTime.Today + TimeSpan.FromDays(1), 124.4f));

                // humidityMeasurements.Add(new HumidityMeasurement("Newcastle upon Tyne", Timestamp.Today - TimeSpan.FromDays(1), 50));
                // humidityMeasurements.Add(new HumidityMeasurement("Newcastle upon Tyne", Timestamp.Today - TimeSpan.FromDays(1), 34.7f));
                // humidityMeasurements.Add(new HumidityMeasurement("Nottingham", Timestamp.Today + TimeSpan.FromDays(1), 74.4f));

                // ignore measurements without a date or location, as they cannot be aggregated
                IEnumerable<PrecipitationMeasurement> pmHasDateAndLoc = precipMeasurements.Where(pm => pm.DateTime.HasValue && pm.Location is not null);
                IEnumerable<TemperatureMeasurement> tmHasDateAndLoc = tempMeasurements.Where(tm => tm.DateTime.HasValue && tm.Location is not null);
                IEnumerable<HumidityMeasurement> hmHasDateAndLoc = humidityMeasurements.Where(hm => hm.Timestamp is not null);

#pragma warning disable CS8629 // Timestamp cannot be null here.
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

                IEnumerable<IGrouping<DateTime?, HumidityMeasurement>> hmByLocation = hmHasDateAndLoc.GroupBy(hm => hm.Timestamp);
                List<HumidityMeasurement> humidityMeasurementsFiltered = new();
                foreach (IGrouping<DateTime?, HumidityMeasurement> group in hmByLocation)
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
                    (pm) => new WeatherMeasurement().GetDataFromPrecipitationMeasurement(pm),
                    (tm) => new WeatherMeasurement().GetDataFromTemperatureMeasurement(tm),
                    (pm, tm) => new WeatherMeasurement().GetDataFromPrecipitationMeasurement(pm)
                                                        .GetDataFromTemperatureMeasurement(tm)
                );


                
                measurements = measurements.FullJoin(
                    humidityMeasurementsFiltered,
                    wm => new { loc = wm.Location, date = wm.DateTime.Value.Date },
                    hm => new { loc = hm.Location, date = hm.Timestamp.Value.Date },
                    (wm) => wm,
                    (hm) => new WeatherMeasurement().GetDataFromHumidityMeasurement(hm),
                    (wm, hm) => wm.GetDataFromHumidityMeasurement(hm) 
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
                // TODO: Get other measurements from other services
                List<HumidityMeasurement> humidityMeasurements = new();

                // ignore measurements without a date, as they cannot be aggregated
                IEnumerable<PrecipitationMeasurement> pmHasDate = precipMeasurements.Where(pm => pm.DateTime.HasValue);
                IEnumerable<TemperatureMeasurement> tmHasDate = tempMeasurements.Where(tm => tm.DateTime.HasValue);
                IEnumerable<HumidityMeasurement> hmHasDate = humidityMeasurements.Where(hm => hm.Timestamp.HasValue);

#pragma warning disable CS8629 // Timestamp cannot be null here.
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
                    (pm) => new WeatherMeasurement().GetDataFromPrecipitationMeasurement(pm),
                    (tm) => new WeatherMeasurement().GetDataFromTemperatureMeasurement(tm),
                    (pm, tm) => new WeatherMeasurement().GetDataFromPrecipitationMeasurement(pm)
                                                        .GetDataFromTemperatureMeasurement(tm)
                );

                measurements = measurements.FullJoin(
                    hmHasDate,
                    wm => new { date = wm.DateTime.Value.Date },
                    hm => new { date = hm.Timestamp.Value.Date },
                    (wm) => wm,
                    (hm) => new WeatherMeasurement().GetDataFromHumidityMeasurement(hm),
                    (wm, hm) => wm.GetDataFromHumidityMeasurement(hm)
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
                    wm.GetDataFromPrecipitationMeasurement(precipMeasurement);
                if (tempMeasurement.DateTime.HasValue && tempMeasurement.DateTime.Value.Date == latestDate)
                    wm.GetDataFromTemperatureMeasurement(tempMeasurement);
                if (humidityMeasurement.Timestamp.HasValue && humidityMeasurement.Timestamp.Value.Date == latestDate)
                    wm.GetDataFromHumidityMeasurement(humidityMeasurement);

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
