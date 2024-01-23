using System.Net.Http;

namespace ClientApp.Data.Repositories
{
    public class TemperatureRepository : IReadOnlyRepository<TemperatureMeasurement>
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public TemperatureRepository(IHttpClientFactory HttpClientFactory)
        {
            _httpClientFactory = HttpClientFactory;
        }

        public async Task<IEnumerable<TemperatureMeasurement>> GetAll()
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                IEnumerable<TemperatureMeasurement>? measurements = await http.GetFromJsonAsync<IEnumerable<TemperatureMeasurement>>("/Temperature/History");

                if (measurements == null)
                    return new List<TemperatureMeasurement>();
                else
                    return measurements;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<TemperatureMeasurement>();
            }
        }

        public async Task<IEnumerable<TemperatureMeasurement>> GetAllByLocation(string location)
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<TemperatureMeasurement>? measurements = await http.GetFromJsonAsync<List<TemperatureMeasurement>>($"/Temperature/History");

                if (measurements == null)
                    return new List<TemperatureMeasurement>();
                else
                    return SelectLocation(measurements, location);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<TemperatureMeasurement>();
            }
        }

        public async Task<TemperatureMeasurement?> GetById(int id)
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                TemperatureMeasurement? measurement = await http.GetFromJsonAsync<TemperatureMeasurement>($"/Temperature/{id}");
                return measurement;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<TemperatureMeasurement?> GetLatestInLocation(string location)
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<TemperatureMeasurement>? measurements = await http.GetFromJsonAsync<List<TemperatureMeasurement>>($"/Temperature");
                if (measurements == null)
                    return null;

                measurements = SelectLocation(measurements, location);
                
                return measurements.MaxBy(m => m.DateTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<TemperatureMeasurement>> GetLatestInEveryLocation()
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("Gateway");
                return await httpClient.GetFromJsonAsync<List<TemperatureMeasurement>>("/Temperature") ?? new List<TemperatureMeasurement>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<TemperatureMeasurement>();
            }
        }

        private List<TemperatureMeasurement> SelectLocation(List<TemperatureMeasurement> mesurements, string location)
        {
            // Process all temperatures to output the relevant temperature data of the given location
            return new List<TemperatureMeasurement>();
        }
    }
}
