using System.Net.Http;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClientApp.Data.Repositories
{
    public class TemperatureRepository : IReadOnlyRepository<TemperatureMeasurement>
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public TemperatureRepository(IHttpClientFactory HttpClientFactory)
        {
            _httpClientFactory = HttpClientFactory;
        }

        public async Task<List<TemperatureMeasurement>?> fetch(HttpClient http, string endpoint){
            return (await http.GetFromJsonAsync<IEnumerable<TemperatureMeasurement.TemperatureData>>(endpoint))
                ?.Select(data => new TemperatureMeasurement(data))?.ToList();
        }

        public async Task<IEnumerable<TemperatureMeasurement>> GetAll()
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                IEnumerable<TemperatureMeasurement>? measurements = await fetch(http, "/Temperature/History");

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
                List<TemperatureMeasurement>? measurements = (await fetch(http, "/Temperature/History"));

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
                TemperatureMeasurement? measurement = new TemperatureMeasurement(await http.GetFromJsonAsync<TemperatureMeasurement.TemperatureData>($"/Temperature/{id}"));
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
                List<TemperatureMeasurement>? measurements = await fetch(http, "/Temperature");
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
                return await fetch(httpClient, "/Temperature") ?? new List<TemperatureMeasurement>();
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
