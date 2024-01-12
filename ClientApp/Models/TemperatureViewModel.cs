namespace ClientApp.Models
{
    public class TemperatureViewModel
    {
        public int Id { get; set; }
        public int SensorID { get; set; } = -1;
        public string SensorDescription { get; set; } = string.Empty;
        public int Temperature { get; set; } = -1000;
        public DateTime Date { get; set; } = DateTime.MinValue;
    }
}
