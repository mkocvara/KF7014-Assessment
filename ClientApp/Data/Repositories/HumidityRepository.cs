using System.Linq;

namespace ClientApp.Data.Repositories
{
    public class HumidityRepository : IReadOnlyRepository<HumidityMeasurement>
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public static int HUMIDITY_MIN
        {
            get; private set;
        }

        public static int HUMIDITY_MAX {
            get; private set;
        }

        public HumidityRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            HUMIDITY_MIN = int.Parse(Environment.GetEnvironmentVariable("HUMIDITY_MIN"));
            HUMIDITY_MAX = int.Parse(Environment.GetEnvironmentVariable("HUMIDITY_MAX"));
        }

        public async Task<IEnumerable<HumidityMeasurement>> GetAll()
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<HumidityMeasurement>? measurements = await http.GetFromJsonAsync<List<HumidityMeasurement>>("/Humidity");

                if (measurements == null)
                    return new List<HumidityMeasurement>(); 
                else
                    return measurements;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<HumidityMeasurement>();
            }
        }

        public async Task<IEnumerable<HumidityMeasurement>> GetAllByLocation(string location)
        {
            List<HumidityMeasurement> measurements = (List<HumidityMeasurement>)await GetAll();
            if (measurements.Count > 0)
            {
                return measurements.FindAll(measurement => 
                            measurement.Location.Equals(location));
            } else return new List<HumidityMeasurement>();
        }

        public async Task<HumidityMeasurement?> GetById(int id)
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                HumidityMeasurement? measurement = await http.GetFromJsonAsync<HumidityMeasurement>($"/Humidity/{id}");
                return measurement;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<HumidityMeasurement>> GetLatestInEveryLocation()
        {
            List<HumidityMeasurement> latestMeasurements = new List<HumidityMeasurement>();
            List<HumidityMeasurement> measurements = (List<HumidityMeasurement>)await GetAll();
            if (measurements.Count <= 0)
            {
                return new List<HumidityMeasurement>();
            }
            var grouped = measurements.GroupBy(m => m.Location);

            foreach ( var group in grouped)
            {
                HumidityMeasurement measurement = group.OrderByDescending(m => m.Timestamp)
                                                       .FirstOrDefault();
                latestMeasurements.Add(measurement);
            }

            return latestMeasurements;
        }

        public async Task<HumidityMeasurement?> GetLatestInLocation(string location)
        {
            List<HumidityMeasurement> measurements = (List<HumidityMeasurement>)await GetAll();
            if (measurements.Count <= 0)
            {
                return new HumidityMeasurement();
            }

            return measurements
                .FindAll(m => m.Location.Equals(location))
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefault();
        }
    }
}
