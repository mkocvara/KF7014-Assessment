namespace ClientApp.Data
{
    // TEMP for testing purposes; to be updated with proper members!
    public class TemperatureMeasurement
    {
        public string? Location { get; set; }
        public DateTime? DateTime { get; set; }
        public float? TemperatureC { get; set; }
        
        public TemperatureMeasurement() { }

        public TemperatureMeasurement(string location, DateTime dateTime, float tempC)
        {
            Location = location;
            DateTime = dateTime;
            TemperatureC = tempC;
        }
    }
}
