using Microsoft.AspNetCore.Components.Routing;
using System.Text.Json;
using System.Timers;

namespace PrecipitationSensors
{
    public class SensorService
    {
        public List<Sensor> Sensors = new();

        public SensorService()
        {
            if (!TryDeserialiseJson())
            {
                // Default sensors if deserialisation fails
                Sensor s = new Sensor("Newcastle upon Tyne", TimeSpan.FromMinutes(1));
                Sensors.Add(s);

                s = new Sensor("Sunderland", TimeSpan.FromMinutes(10));
                Sensors.Add(s);

                s = new Sensor("Durham", TimeSpan.FromMinutes(30));
                Sensors.Add(s);
            }
        }

        // Deserialises sensors.json and adds the sensors to the list of sensors.
        // Returns false if deserialisation fails, true otherwise.
        private bool TryDeserialiseJson()
        {
            try
            {
                string fileName = "sensors.json";
                string jsonString = File.ReadAllText(fileName);

                if (string.IsNullOrEmpty(jsonString))
                    return false;

                IEnumerable<SensorInitData>? initData = JsonSerializer.Deserialize<IEnumerable<SensorInitData>>(jsonString);

                if (initData == null || !initData.Any())
                    return false;

                foreach (SensorInitData data in initData)
                {
                    Sensor s = new(data);
                    Sensors.Add(s);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
