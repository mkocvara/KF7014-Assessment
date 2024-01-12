namespace ClientApp.Data
{
    public class TemperatureMeasurement
    {
        public int Id { get; set; }
        public int SensorID { get; set; } = -1;
        public string? Location { get; set; } = string.Empty;
        public int? Temperature { get; set; } = -1000;
        public DateTime? DateTime { get; set; }

        public TemperatureMeasurement() { }

        public TemperatureMeasurement(string location, DateTime dateTime, float tempC)
        {
            Location = location;
            DateTime = dateTime;
            Temperature = (int)tempC;
        }
    }
}
