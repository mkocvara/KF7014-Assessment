using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Pages.Components
{
    public class LiveTemperatureViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public LiveTemperatureViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        //public IViewComponentResult Invoke()
        //{
        //    return View();
        //}

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Make an HTTP request to the API endpoint
            var apiEndpoint = "https://localhost:7081/Temperature";
            var response = await _httpClient.GetAsync(apiEndpoint);

            Console.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response
                // - The data being pulled is the historical data (unhinged)
                // I must create another function on the API that only gathers
                // the latest values for each sensor.
                // - I must create model classes to recieve both every historical
                // data point and live temperature data
                // - add a refresh every 3 min for all the data. See if it is
                // possible to listen to data updates from the API.

                Console.WriteLine(response.Content.ToString());

                
                return View();
            }

            // Handle the case where the API request is not successful
            return Content("Unable to fetch recent posts");
        }
    }
}
