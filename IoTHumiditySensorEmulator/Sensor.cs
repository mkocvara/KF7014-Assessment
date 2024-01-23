using System.Text;
using System.Text.Json.Nodes;

namespace IoTHumiditySensorEmulator
{
    internal class Sensor
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private int SendInterval { get; set; } // measured in seconds

        private int ThreadID { get; set; }

        public Sensor(int threadID)
        {
            List<int> SendIntervalOptions = new List<int>()
            { // 30 minutes, 1h and 3h respectively
                60*30, 60*60, 60*60*3
            };

            Random rng = new Random();
            Latitude = rng.NextDouble() * 180 - 90; ;
            Longitude = rng.NextDouble() * 360 - 180; ;
            SendInterval = SendIntervalOptions[rng.Next() % 3];
            ThreadID = threadID;
        }

        public async Task SendRequests()
        {
            while (true)
            {
                string URL = Environment.GetEnvironmentVariable("TARGET_URL");
                if (URL == null)
                {
                    Console.WriteLine("TARGET_URL is not set. Exiting...");
                    Environment.Exit(1);
                }

                // NEW SNIPPET HERE FOR SSL CHECK

                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                // END SNIPPET
                // Make sure you delete the reference below as well
                using (HttpClient client = new HttpClient(handler))
                {
                    Random rng = new Random();

                    IoTHumidityDataCollector.Models.HumidityReading hr = new IoTHumidityDataCollector.Models.HumidityReading();

                    DateTime timestamp = DateTime.UtcNow;
                    double percentage = rng.NextDouble() * 100;
                    JsonObject jsonReading = new JsonObject
                    {
                        { "timestamp", timestamp },
                        { "percentage", percentage },
                        { "latitude", Latitude },
                        { "longitude", Longitude }
                    };

                    JsonArray jsonArray = new JsonArray
                    {
                        jsonReading
                    };

                    try
                    {
                        HttpResponseMessage result = await client.PostAsync(URL, new StringContent(jsonArray.ToString(), Encoding.UTF8, "application/json"));
                        Console.WriteLine($"Thread {ThreadID}: ({timestamp}) - {percentage}% @ {Latitude},{Longitude}; STATUS: {result.StatusCode}");
                        if (result.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine(await result.Content.ReadAsStringAsync());
                        }

                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                int sleeptime = SendInterval * 1000;

                // Reduce sleep time because this is a development build
                sleeptime /= 600;
                Thread.Sleep(sleeptime);
            }
        }
    }
}
