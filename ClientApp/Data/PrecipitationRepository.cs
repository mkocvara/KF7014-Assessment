using static System.Net.WebRequestMethods;

namespace ClientApp.Data
{
    public class PrecipitationRepository : IReadOnlyRepository<PrecipitationMeasurement>
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public PrecipitationRepository(IHttpClientFactory HttpClientFactory)
        {
            _httpClientFactory = HttpClientFactory;
        }

        public async Task<IEnumerable<PrecipitationMeasurement>> GetAll()
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<PrecipitationMeasurement>? measurements = await http.GetFromJsonAsync<List<PrecipitationMeasurement>>("/Precipitation");
            
                if (measurements == null)
                    return new List<PrecipitationMeasurement>();
                else
                    return measurements;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<PrecipitationMeasurement>();
            }
        }

        public async Task<IEnumerable<PrecipitationMeasurement>> GetAllByLocation(string location)
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<PrecipitationMeasurement>? measurements = await http.GetFromJsonAsync<List<PrecipitationMeasurement>>($"/Precipitation/location/{location}");

                if (measurements == null)
                    return new List<PrecipitationMeasurement>();
                else
                    return measurements;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<PrecipitationMeasurement>();
            }
        }

        public async Task<PrecipitationMeasurement?> GetById(int id)
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                PrecipitationMeasurement? measurement = await http.GetFromJsonAsync<PrecipitationMeasurement>($"/Precipitation/{id}");
                return measurement;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<PrecipitationMeasurement?> GetLatest()
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<PrecipitationMeasurement>? measurements = await http.GetFromJsonAsync<List<PrecipitationMeasurement>>($"/Precipitation");
                if (measurements == null)
                    return null;

                int latestIndex = 0;
                for(int i = 1; i < measurements.Count; i++)
                {
                    if (measurements[i].DateTime > measurements[latestIndex].DateTime)
                        latestIndex = i;
                }

                return measurements[latestIndex];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<PrecipitationMeasurement>> GetLatestInEveryLocation()
        {
            try
            {
                HttpClient http = _httpClientFactory.CreateClient("Gateway");
                List<PrecipitationMeasurement>? measurements = await http.GetFromJsonAsync<List<PrecipitationMeasurement>>($"/Precipitation");

                if (measurements == null)
                    return new List<PrecipitationMeasurement>();

                List<PrecipitationMeasurement> latestMeasurements = new List<PrecipitationMeasurement>();
                var mByLocation = measurements.GroupBy(m => m.Location);

                int latestIndex;
                foreach(var m in mByLocation)
                {
                    latestIndex = 0;
                    for (int i = 1; i < m.Count(); i++)
                    {
                        if (m.ElementAt(i).DateTime > m.ElementAt(latestIndex).DateTime)
                            latestIndex = i;
                    }
                    latestMeasurements.Add(m.ElementAt(latestIndex));
                }

                return latestMeasurements;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<PrecipitationMeasurement>();
            }
        }
    }
}
