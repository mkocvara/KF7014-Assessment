using System.Collections.Generic;
using TemperatureAPI.Temperature.Models;
using Microsoft.EntityFrameworkCore;

namespace TemperatureAPI.Temperature.Data
{
    public class DataEntry
    {
        public int Id { get; set; }
        public int Temperature { get; set; }
        public int SensorId { get; set; }
        public String SensorDescription { get; set; }
        public String Date { get; set; }

        public void CopyFrom(DataEntry entry)
        {
            Id = entry.Id;
            Temperature = entry.Temperature;
            SensorId = entry.SensorId;
            SensorDescription = entry.SensorDescription;
            Date = entry.Date;
        }
    }
}