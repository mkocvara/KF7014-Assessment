using System.Diagnostics;
using TemperatureAPI.Sensor.Data;

namespace TemperatureAPI.Sensor.Services
{
    public class PeriodicSender : BackgroundService
    {
        private PeriodicTimer _timer;
        private readonly Data.Sensor _sensor;

        public PeriodicSender(Data.Sensor sensor)
        {
            Debug.WriteLine("Waiting..............................................");
            _sensor = sensor;
            _timer = new(TimeSpan.FromSeconds(sensor.IntervalInMinutes));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await _sensor.SendTemperature();
            }
        }
    }
}
