using ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                // - I must create model classes to recieve both every historical
                // data points and live temperature data
                // - Create the graph or table? with the data?

                string data = response.Content.ReadAsStringAsync().Result;
                List<TemperatureViewModel> latestTemperatures = JsonConvert.DeserializeObject<List<TemperatureViewModel>>(data) ?? new List<TemperatureViewModel>();

                return View(latestTemperatures);
            }

            // Handle the case where the API request is not successful
            return Content("Unable to fetch recent posts");
        }
    }
}
