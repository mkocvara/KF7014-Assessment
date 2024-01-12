namespace ClientApp.Data
{
    // TEMP for testing purposes; to be updated with proper members!
    public class HumidityMeasurement
    {
        public string? Location { get; set; }
        public DateTime? DateTime { get; set; }
        public float? Humidity { get; set; }
        
        public HumidityMeasurement() { }

        public HumidityMeasurement(string location, DateTime dateTime, float humidity)
        {
            Location = location;
            DateTime = dateTime;
            Humidity = humidity;
        }
    }
}
