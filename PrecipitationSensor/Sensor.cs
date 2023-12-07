using System.Timers;

namespace PrecipitationSensors
{
    public class Sensor
    {
        public PrecipitationMeasurementDTO NextMeasurement = default!;
        public readonly string Location = "TestLocation";
        public readonly System.Timers.Timer Timer = new System.Timers.Timer(1000);

        private DateTime _lastDataSentTime;
        private TimeSpan _dataSendInterval = TimeSpan.FromMinutes(30);
        private static readonly HttpClient _http = new();

        public Sensor(string location, TimeSpan interval)
        {
            _dataSendInterval = interval;
            Location = location;

            Timer.AutoReset = true;
            Timer.Elapsed += SecondTick;
            Timer.Enabled = true;

            _http.BaseAddress = new Uri("https://localhost:7081"); // Gateway URI

            NewMeasurement();
        }

        public string GetTimeUntilSendAsString()
        {
            TimeSpan timeTo = (_lastDataSentTime + _dataSendInterval) - DateTime.Now;
            return timeTo.ToString(@"mm\:ss");
        }

        public void SetDataSendInterval(TimeSpan interval)
        {
            _dataSendInterval = interval;
        }

        private void NewMeasurement()
        {
            NextMeasurement = PrecipitationMeasurementDTO.GetTestMeasurement();
            NextMeasurement.Location = Location;
            if (NextMeasurement.DateTime.HasValue)
                NextMeasurement.DateTime = NextMeasurement.DateTime + _dataSendInterval;
            _lastDataSentTime = DateTime.Now;
        }

        private void SecondTick(object? sender, ElapsedEventArgs e)
        {
            if (DateTime.Now > _lastDataSentTime + _dataSendInterval)
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
