namespace TemperatureAPI.Temperature.Models
{
    public class TemperatureModel
    {
        public int Id { get; set; }
        public int Temperature { get; set; }
        public int SensorID { get; set; }
        public string SensorDescription { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
