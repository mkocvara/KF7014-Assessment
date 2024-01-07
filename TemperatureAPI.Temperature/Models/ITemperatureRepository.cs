using System.Collections.Generic;
using System.Threading.Tasks;
using TemperatureAPI.Temperature.Data;

namespace TemperatureAPI.Temperature.Models
{
    public interface ITemperatureRepository
    {
        public Task<List<TemperatureModel>> GetAllTemperatureEntries();
        public Task<TemperatureModel?> GetTemperatureEntry(int id);
        public Task<DataEntry> AddTemperatureEntry(DataEntry entry);
        public Task<DataEntry> UpdateTemperatureEntry(int id, DataEntry entry);
        public Task DeleteTemperatureEntry(int id);
    }
}