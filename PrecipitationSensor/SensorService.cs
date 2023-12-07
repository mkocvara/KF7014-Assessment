using System.Timers;

namespace PrecipitationSensors
{
    public class SensorService
    {
        public List<Sensor> Sensors = new();

        public SensorService()
        {
            Sensor s = new Sensor("Newcastle upon Tyne", TimeSpan.FromMinutes(1));
            Sensors.Add(s);

            s = new Sensor("Sunderland", TimeSpan.FromMinutes(10));
            Sensors.Add(s);

            s = new Sensor("Durham", TimeSpan.FromMinutes(30));
            Sensors.Add(s);
        }
    }
}
