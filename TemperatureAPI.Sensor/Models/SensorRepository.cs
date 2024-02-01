using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System.Text.Json;
using TemperatureAPI.Sensor.Data;

namespace TemperatureAPI.Sensor.Models
{
    public class SensorRepository : ISensorRepository
    {
        private readonly List<Data.Sensor> sensors = new();
        private int nextId = 1;

        private static List<string> Locations = new List<string>() {
            "Newcastle upon Tyne",
            "Sunderland",
            "Durham",
            "Middlesborough",
            "Liverpool",
            "Manchester",
            "London",
            "Oxford",
            "Cambridge",
            "Birmingham",
            "Canterbury",
            "Nottingham",
            "Derby",
            "Bath",
            "Worcester",
            "York",
            "Stoke-on-Trent",
            "Leicester",
            "Sheffield",
            "Leeds",
            "Chester",
            "Exeter",
            "Carlisle",
            "Norwich",
            "Bradford",
            "Coventry",
            "Runcorn",
            "Portsmouth",
            "Bristol",
            "Plymouth"
        };

        public SensorRepository()
        {
            init();
            Console.WriteLine($"Initialized Sensor Repository");
        }

        public void init()
        {
            Debug.WriteLine("Sensor repository..............................................");

            try
            {
                Random rng = new();
                for(int i=0; i<30; i++)
                {
                    sensors.Add(new Data.Sensor(nextId++, Locations[rng.Next() % Locations.Count]));
                }

                Debug.WriteLine("Successfully Deserialized 30 sensors from json!");

            }
            catch (Exception e)
            {
                Console.WriteLine($"An exception occured: {e.Message}");
                for (int id = 1; id <= 30; id++)
                {
                    sensors.Add(new Data.Sensor(nextId++,
                        "Northumberland Street, Newcastle Upon Tyne"));
                }
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

        // Deserialises sensors.json and adds the sensors to the list of sensors.
        // Returns false if deserialisation fails, true otherwise.
        /*
        private bool TryDeserialiseJson()
        {
            try
            {
                string fileName = "sensors.json";
                string jsonString = File.ReadAllText(fileName);

                if (string.IsNullOrEmpty(jsonString))
                    return false;

                IEnumerable<Data.Sensor.InitData>? initData = JsonSerializer.Deserialize<IEnumerable<Data.Sensor.InitData>>(jsonString);

                if (initData == null || !initData.Any())
                    return false;

                foreach (Data.Sensor.InitData data in initData)
                {
                    Data.Sensor s = new(data);
                    Sensors.Add(s);
                }

                return true;
            }
            catch
            {
                return false;
            }
        } */ 
    }


}
