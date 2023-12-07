using System.Timers;

namespace PrecipitationSensor
{
    public class SensorService
    {
        public PrecipitationMeasurementDTO NextMeasurement = default!;
        public DateTime LastDataSentTime;
        
        public static readonly System.Timers.Timer Timer = new System.Timers.Timer(1000);

        private static readonly TimeSpan _dataSendInterval = TimeSpan.FromMinutes(30);
        private static readonly HttpClient _http = new();

        public SensorService()
        {
            NewMeasurement();

            Timer.AutoReset = true;
            Timer.Elapsed += SecondTick;
            Timer.Enabled = true;

            _http.BaseAddress = new Uri("https://localhost:7081"); // Gateway URI
        }

        public string GetTimeUntilSendAsString()
        {
            TimeSpan timeTo = (LastDataSentTime + _dataSendInterval) - DateTime.Now;
            return timeTo.ToString(@"mm\:ss");
        }

        private void NewMeasurement()
        {
            NextMeasurement = PrecipitationMeasurementDTO.GetTestMeasurement();
            LastDataSentTime = DateTime.Now;
        }

        private void SecondTick(object? sender, ElapsedEventArgs e)
        {
            if (DateTime.Now > LastDataSentTime + _dataSendInterval)
            {
                SendData();
                NewMeasurement();
            }

            Console.WriteLine("Tick! Time left: " + GetTimeUntilSendAsString());
        }

        private void SendData()
        {
            var response = _http.PostAsJsonAsync("/Precipitation", NextMeasurement)
                            .ContinueWith((response) => Console.Out.WriteLine("Response received, status code: " + response.Result.StatusCode));
        }
    }
}
