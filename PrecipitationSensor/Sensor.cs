using System.Timers;

namespace PrecipitationSensors
{
    public class Sensor
    {
        public struct InitData
        {
            public string Location { get; set; }
            public float Interval { get; set; }
        }

        public PrecipitationMeasurementDTO NextMeasurement { get => _nextMeasurement; }
        public readonly string Location;

        private PrecipitationMeasurementDTO _nextMeasurement = default!;
        private readonly System.Timers.Timer _timer;
        private DateTime _lastDataSentTime;
        private TimeSpan _dataSendInterval = TimeSpan.FromMinutes(1);
        private static readonly HttpClient _http = new() { BaseAddress = new Uri( Environment.GetEnvironmentVariable("TARGET_URL") ?? "https://localhost:7081") }; // Gateway URI

        public Sensor(string location, TimeSpan interval)
        {
            _dataSendInterval = interval;
            Location = location;

            _timer = new System.Timers.Timer(_dataSendInterval.TotalMilliseconds);
            _timer.AutoReset = true;
            _timer.Elapsed += OnIntervalElapsed;
            _timer.Enabled = true;

            NewMeasurement();
        }

        // Constructor using SensorInitData
        public Sensor(InitData initData) 
            : this(initData.Location, TimeSpan.FromMinutes(initData.Interval))
        {
        }

        /// <summary>
        /// Get time until next measurement formatted as dd:hh:mm:ss.
        /// </summary>
        /// <returns>Time until the next measurement is sent as a formatted string.</returns>
        public string GetTimeUntilSendAsString()
        {
            TimeSpan timeTo = (_lastDataSentTime + _dataSendInterval) - DateTime.Now;

            string timeFormat = "";
            timeFormat += (timeTo.Days > 0) ? @"dd\:" : "";
            timeFormat += (timeTo.Hours > 0) ? @"hh\:" : "";
            timeFormat += @"mm\:ss";

            return timeTo.ToString(timeFormat);
        }

        /// <summary>
        /// Sets the interval between individual measurements.
        /// </summary>
        /// <param name="interval">Interval to set.</param>
        public void SetDataSendInterval(TimeSpan interval)
        {
            _dataSendInterval = interval;
            _timer.Interval = _dataSendInterval.TotalMilliseconds;
        }

        // Generates the next measurement to be sent.
        private void NewMeasurement()
        {
            _nextMeasurement = PrecipitationMeasurementDTO.GetTestMeasurement();
            _nextMeasurement.Location = Location;
            _nextMeasurement.DateTime = DateTime.Now + _dataSendInterval;
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
