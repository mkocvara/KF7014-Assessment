using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemperatureAPI.Sensor.Models
{
    public interface ISensorRepository
    {
        public void init();
        public List<SensorModel> GetAllSensors();
        public SensorModel GetSensor(int id);
        public SensorModel AddSensor(SensorModel sensor);
        public SensorModel UpdateSensor(int id, SensorModel sensor);
        public void DeleteSensor(int id);
    }
}


