using System.Timers;

namespace PrecipitationSensors
{
    public struct SensorInitData
    {
        public string Location { get; set; }
        public float Interval { get; set; }
    }


    public class Sensor
    {
        public PrecipitationMeasurementDTO NextMeasurement = default!;
        public readonly string Location;
        public readonly System.Timers.Timer Timer;

        private DateTime _lastDataSentTime;
        private TimeSpan _dataSendInterval = TimeSpan.FromMinutes(1);
        private static readonly HttpClient _http = new();

        public Sensor(string location, TimeSpan interval)
        {
            _dataSendInterval = interval;
            Location = location;

            Timer = new System.Timers.Timer(_dataSendInterval.TotalMilliseconds);
            Timer.AutoReset = true;
            Timer.Elapsed += OnIntervalElapsed;
            Timer.Enabled = true;

            _http.BaseAddress = new Uri("https://localhost:7081"); // Gateway URI

            NewMeasurement();
        }

        // Constructor using SensorInitData
        public Sensor(SensorInitData initData) 
            : this(initData.Location, TimeSpan.FromMinutes(initData.Interval))
        {
        }

        /// <summary>
        /// Get time until next measurement is sent formatted as hh:mm:ss orr mm:ss if 0 hours.
        /// </summary>
        /// <returns>Time until the next measurement is sent as a formatted string.</returns>
        public string GetTimeUntilSendAsString()
        {
            TimeSpan timeTo = (_lastDataSentTime + _dataSendInterval) - DateTime.Now;
            return (timeTo.Hours > 0) ? timeTo.ToString(@"hh\:mm\:ss") : timeTo.ToString(@"mm\:ss");
        }

        /// <summary>
        /// Sets the interval between individual measurements.
        /// </summary>
        /// <param name="interval">Interval to set.</param>
        public void SetDataSendInterval(TimeSpan interval)
        {
            _dataSendInterval = interval;
            Timer.Interval = _dataSendInterval.TotalMilliseconds;
        }

        // Generates the next measurement to be sent.
        private void NewMeasurement()
        {
            NextMeasurement = PrecipitationMeasurementDTO.GetTestMeasurement();
            NextMeasurement.Location = Location;
            NextMeasurement.DateTime = DateTime.Now + _dataSendInterval;
            _lastDataSentTime = DateTime.Now;
        }

        // Event handler for timer tick. Sends data and generates new measurement.
        private void OnIntervalElapsed(object? sender, ElapsedEventArgs e)
        {
            SendData();
            NewMeasurement();
        }

        // Send data to the precipitation microservice through the gateway.
        private void SendData()
        {
            var response = _http.PostAsJsonAsync("/Precipitation", NextMeasurement)
                            .ContinueWith((response) => Console.Out.WriteLine("Response received, status code: " + response.Result.StatusCode));
        }
    }
}
