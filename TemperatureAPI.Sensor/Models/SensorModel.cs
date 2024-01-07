using TemperatureAPI.Sensor.Data;

namespace TemperatureAPI.Sensor.Models
{
    public class SensorModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public int IntervalInMinutes { get; set; }

        public void FromSensor(Data.Sensor sensor)
        {
            Id = sensor.Id;
            Description = sensor.Description;
            IntervalInMinutes = sensor.IntervalInMinutes;
        }
    }
}
