using ClientApp.Data;
using static MoreLinq.Extensions.FullGroupJoinExtension;
using static MoreLinq.Extensions.FullJoinExtension;

namespace ClientApp.Data.Repositories
{
    public class AggregateRepository : IReadOnlyRepository<WeatherMeasurement>
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public AggregateRepository(IHttpClientFactory HttpClientFactory)
        {
            _httpClientFactory = HttpClientFactory;
        }

        public async Task<IEnumerable<WeatherMeasurement>> GetAll()
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<PrecipitationMeasurement>? precipMeasurements = await http.GetFromJsonAsync<List<PrecipitationMeasurement>>("/Precipitation");
                // TODO: Get other measurements from other services
                List<TemperatureMeasurement>? tempMeasurements = new();
                List<HumidityMeasurement>? humidityMeasurements = new();

                // avoid null references
                precipMeasurements ??= new List<PrecipitationMeasurement>();
                tempMeasurements ??= new List<TemperatureMeasurement>();
                humidityMeasurements ??= new List<HumidityMeasurement>();

                // DEBUG TEMP
                tempMeasurements.Add(new TemperatureMeasurement("Newcastle upon Tyne", DateTime.Today - TimeSpan.FromDays(1), 10));
                tempMeasurements.Add(new TemperatureMeasurement("Leeds", DateTime.Today - TimeSpan.FromDays(2), 154.4f));
                tempMeasurements.Add(new TemperatureMeasurement("Nottingham", DateTime.Today + TimeSpan.FromDays(1), 124.4f));

                humidityMeasurements.Add(new HumidityMeasurement("Newcastle upon Tyne", DateTime.Today - TimeSpan.FromDays(1), 50));
                humidityMeasurements.Add(new HumidityMeasurement("Newcastle upon Tyne", DateTime.Today - TimeSpan.FromDays(1), 34.7f));
                humidityMeasurements.Add(new HumidityMeasurement("Nottingham", DateTime.Today + TimeSpan.FromDays(1), 74.4f));

                // ignore measurements without a date or location, as they cannot be aggregated
                IEnumerable<PrecipitationMeasurement> pmHasDateAndLoc = precipMeasurements.Where(pm => pm.DateTime.HasValue && pm.Location is not null);
                IEnumerable<TemperatureMeasurement> tmHasDateAndLoc = tempMeasurements.Where(tm => tm.DateTime.HasValue && tm.Location is not null);
                IEnumerable<HumidityMeasurement> hmHasDateAndLoc = humidityMeasurements.Where(hm => hm.DateTime.HasValue && hm.Location is not null);

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

                IEnumerable<IGrouping<string?, TemperatureMeasurement>> tmByLocation = tmHasDateAndLoc.GroupBy(pm => pm.Location);
                List<TemperatureMeasurement> tempMeasurementsFiltered = new();
                foreach (IGrouping<string?, TemperatureMeasurement> group in tmByLocation)
                {
                    IEnumerable<TemperatureMeasurement> onePerDay = group.GroupBy(pm => pm.DateTime.Value.Date)
                                                                           .Select(g => g.OrderBy(pm => pm.DateTime)
                                                                                         .Last());
                    tempMeasurementsFiltered.AddRange(group);
                }

                IEnumerable<IGrouping<string?, HumidityMeasurement>> hmByLocation = hmHasDateAndLoc.GroupBy(pm => pm.Location);
                List<HumidityMeasurement> humidityMeasurementsFiltered = new();
                foreach (IGrouping<string?, HumidityMeasurement> group in hmByLocation)
                {
                    IEnumerable<HumidityMeasurement> onePerDay = group.GroupBy(pm => pm.DateTime.Value.Date)
                                                                           .Select(g => g.OrderBy(pm => pm.DateTime)
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
                    hm => new { loc = hm.Location, date = hm.DateTime.Value.Date },
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
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<PrecipitationMeasurement>? precipMeasurements = await http.GetFromJsonAsync<List<PrecipitationMeasurement>>($"/Precipitation/location/{location}");
                // TODO: Get other measurements from other services
                List<TemperatureMeasurement>? tempMeasurements = new();
                List<HumidityMeasurement>? humidityMeasurements = new();

                // avoid null references
                precipMeasurements ??= new List<PrecipitationMeasurement>();
                tempMeasurements ??= new List<TemperatureMeasurement>();
                humidityMeasurements ??= new List<HumidityMeasurement>();

                // ignore measurements without a date, as they cannot be aggregated
                IEnumerable<PrecipitationMeasurement> pmHasDate = precipMeasurements.Where(pm => pm.DateTime.HasValue);
                IEnumerable<TemperatureMeasurement> tmHasDate = tempMeasurements.Where(tm => tm.DateTime.HasValue);
                IEnumerable<HumidityMeasurement> hmHasDate = humidityMeasurements.Where(hm => hm.DateTime.HasValue);

#pragma warning disable CS8629 // DateTime cannot be null here.
                // where measurements share the same date, keep only the most recent
                pmHasDate = pmHasDate.GroupBy(pm => pm.DateTime.Value.Date)
                                                 .Select(g => g.OrderBy(pm => pm.DateTime)
                                                               .Last());
                tmHasDate = tmHasDate.GroupBy(tm => tm.DateTime.Value.Date)
                                                 .Select(g => g.OrderBy(tm => tm.DateTime)
                                                               .Last());
                hmHasDate = hmHasDate.GroupBy(hm => hm.DateTime.Value.Date)
                                                 .Select(g => g.OrderBy(hm => hm.DateTime)
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
                    hm => new { date = hm.DateTime.Value.Date },
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
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                
                List<PrecipitationMeasurement>? precipMeasurements = await http.GetFromJsonAsync<List<PrecipitationMeasurement>>($"/Precipitation/location/{location}");
                // TODO: Get other measurements from other services
                List<TemperatureMeasurement>? tempMeasurements = new();
                List<HumidityMeasurement>? humidityMeasurements = new();

                // avoid null references
                precipMeasurements ??= new List<PrecipitationMeasurement>();
                tempMeasurements ??= new List<TemperatureMeasurement>();
                humidityMeasurements ??= new List<HumidityMeasurement>();

                // ignore measurements without a date, as they cannot be aggregated
                IEnumerable<PrecipitationMeasurement> pmHasDate = precipMeasurements.Where(pm => pm.DateTime.HasValue);
                IEnumerable<TemperatureMeasurement> tmHasDate = tempMeasurements.Where(tm => tm.DateTime.HasValue);
                IEnumerable<HumidityMeasurement> hmHasDate = humidityMeasurements.Where(hm => hm.DateTime.HasValue);

                PrecipitationMeasurement? pmLatest = pmHasDate.MaxBy(pm => pm.DateTime);
                TemperatureMeasurement? tmLatest = tmHasDate.MaxBy(tm => tm.DateTime);
                HumidityMeasurement? hmLatest = hmHasDate.MaxBy(hm => hm.DateTime);

                if (pmLatest == null || tmLatest == null || hmLatest == null)
                    throw new Exception($"Error: failed to find one or more latest measurements in location \"{location}\"");

#pragma warning disable CS8629 // DateTime cannot be null here.
                // Find latest date of measurement
                DateTime latestDate = new List<DateTime> { pmLatest.DateTime.Value.Date, tmLatest.DateTime.Value.Date, hmLatest.DateTime.Value.Date }.Max();
#pragma warning restore CS8629

                WeatherMeasurement wm = new();
                if (pmLatest.DateTime.Value.Date == latestDate)
                    wm.GetDataFromPrecipitationMeasurement(pmLatest);
                if (tmLatest.DateTime.Value.Date == latestDate)
                    wm.GetDataFromTemperatureMeasurement(tmLatest);
                if (hmLatest.DateTime.Value.Date == latestDate)
                    wm.GetDataFromHumidityMeasurement(hmLatest);

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
