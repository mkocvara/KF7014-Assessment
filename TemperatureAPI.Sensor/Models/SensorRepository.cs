using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using TemperatureAPI.Sensor.Data;

namespace TemperatureAPI.Sensor.Models
{
    public class SensorRepository : ISensorRepository
    {
        private readonly List<Data.Sensor> sensors = new();
        private int nextId = 1;

        public SensorRepository()
        {
            init();
        }

        public void init()
        {
            Debug.WriteLine("Sensor repository..............................................");

            for (int id = 1; id <= 30; id++)
            {
                sensors.Add(new Data.Sensor(nextId++,
                    "Northumberland Street, Newcastle Upon Tyne"));
            }
        }

        public List<SensorModel> GetAllSensors()
        {
            return sensors.Select(
                sensor => new SensorModel {
                    Id = sensor.Id,
                    Description = sensor.Description,
                    IntervalInMinutes = sensor.IntervalInMinutes })
                .ToList();
        }

        public SensorModel? GetSensor(int id)
        {
            Data.Sensor? sensor = sensors.FirstOrDefault(sensor => sensor.Id == id);
            if (sensor == null)
            {
                return null;
            }

            return new SensorModel() { 
                Id = sensor.Id,
                Description = sensor.Description, 
                IntervalInMinutes = sensor.IntervalInMinutes };
        }

        public SensorModel AddSensor(SensorModel sensor)
        {
            Data.Sensor newSensor = new Data.Sensor(
                nextId++, 
                sensor.Description,
                sensor.IntervalInMinutes);
            sensors.Add(newSensor);

            SensorModel newSensorModel = new SensorModel();
            newSensorModel.FromSensor(newSensor);
            return newSensorModel;
        }

        public SensorModel? UpdateSensor(int id, SensorModel newSensor)
        {
            Data.Sensor? sensor = sensors.FirstOrDefault(s => s.Id == id);
            if (sensor == null)
            {
                return null;
            }

            sensor.CopyFrom(
                new Data.Sensor(
                    sensor.Id,
                    sensor.Description, 
                    sensor.IntervalInMinutes)
                );
            sensor.updateTimer();

            return new SensorModel()
            {
                Id = sensor.Id,
                Description = sensor.Description,
                IntervalInMinutes = sensor.IntervalInMinutes
            };
        }

        public void DeleteSensor(int id) {
            Data.Sensor? sensor = sensors.FirstOrDefault(s => s.Id == id);

            if (sensor is null)
            {
                return;
            }
            sensors.Remove(sensor);
        }
    }
}
