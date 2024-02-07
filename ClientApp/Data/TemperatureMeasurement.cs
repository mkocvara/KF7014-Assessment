using System;

namespace ClientApp.Data
{
    public class TemperatureMeasurement
    {
        public class TemperatureData
        {
            public int Id { get; set; }
            public int SensorID { get; set; } = -1;
            public string? SensorDescription { get; set; } = string.Empty;
            public float? Temperature { get; set; } = -1000f;
            public DateTime? date { get; set; }

        }

        public int Id { get; set; }
        public int SensorID { get; set; } = -1;
        public string? Location { get; set; } = string.Empty;
        public float? Temperature { get; set; } = -1000f;
        public DateTime? DateTime { get; set; }

        public TemperatureMeasurement() { }

        public TemperatureMeasurement(TemperatureData? data)
        {
            if (data != null)
            {
                Id = data.Id;
                SensorID = data.SensorID;
                Location = data.SensorDescription;
                DateTime = data.date;
                Temperature = data.Temperature;
            }
        }

        public TemperatureMeasurement(string location, DateTime dateTime, float tempC)
        {
            Location = location;
            DateTime = dateTime;
            Temperature = tempC;
        }

        public override string ToString() {
            return $"Temperature {Temperature} at location: {Location} at DateTime: {DateTime}";
        }
    }

    
}
