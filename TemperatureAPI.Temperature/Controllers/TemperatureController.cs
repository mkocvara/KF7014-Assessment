using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using TemperatureAPI.Temperature.Data;
using TemperatureAPI.Temperature.Filters;
using TemperatureAPI.Temperature.Models;

namespace TemperatureAPI.Temperature.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly IBus _eventBus;
        public TemperatureController(ITemperatureRepository temperatureRepository, IBus eventBus)
        {
            _temperatureRepository = temperatureRepository;
            _eventBus = eventBus;
        }

        [HttpGet]
        public async Task<ActionResult<List<TemperatureModel>>> GetLatestTemperatureEntries()
        {
            return await _temperatureRepository.GetLatestTemperatureEntries();
        }

        [HttpGet("History")]
        public async Task<ActionResult<List<TemperatureModel>>> GetHistoricalTemperatureEntries()
        {
            return await _temperatureRepository.GetHistoricalTemperatureEntries();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TemperatureModel>> GetTemperatureEntry(int id)
        {
            TemperatureModel? model = await _temperatureRepository.GetTemperatureEntry(id);
            if (model == null)
            {
                return NotFound();
            }
            return model;
        }

        [APIKeyAuth("sensor")]
        [HttpPost]
        public async Task<ActionResult<TemperatureModel>> CreateTemperatureEntry(DataEntry entry)
        {
            DataEntry createdEntry = await _temperatureRepository.AddTemperatureEntry(entry);

            // Checks for extreme weather events
            String? maxTemperatureString = Environment.GetEnvironmentVariable("MAX_TEMPERATURE");
            String? minTemperatureString = Environment.GetEnvironmentVariable("MIN_TEMPERATURE");

            bool isExtremeWeather = false;
            if (maxTemperatureString is not null && createdEntry.Temperature > Int32.Parse(maxTemperatureString))
            {
                Console.WriteLine("ALERT!!\nSensor {0} has triggered an Extreme Weather Alert." +
                    "\nRecorded temperature is greater than the maximum threshold: {1} > {2}\n",
                    createdEntry.SensorId, createdEntry.Temperature, Int32.Parse(maxTemperatureString));
                isExtremeWeather = true;
            }

            if (minTemperatureString is not null && createdEntry.Temperature < Int32.Parse(minTemperatureString))
            {
                Console.WriteLine("ALERT!!\nSensor {0} has triggered an Extreme Weather Alert." +
                    "\nRecorded temperature is lower than the minimum threshold: {1} < {2}\n",
                    createdEntry.SensorId, createdEntry.Temperature, Int32.Parse(minTemperatureString));
                isExtremeWeather = true;
            }

            // Publish the new measurement's id as a message to the event bus
            try
            {
                if (isExtremeWeather)
                    _eventBus.PubSub.Publish(createdEntry.Id, "Temperature");
            }
            catch
            {
                Console.WriteLine($"Failed to publish message to event bus. Very possibly, RabbitMQ is not running.");
            }

            // Returns confirmation of creation
            return CreatedAtAction(nameof(GetTemperatureEntry), new { id = createdEntry.Id }, createdEntry);
        }

        [APIKeyAuth("admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemperatureEntry(int id, DataEntry newEntry)
        {
            if (id != newEntry.Id)
                return BadRequest();

            TemperatureModel? entry = await _temperatureRepository.GetTemperatureEntry(id);

            if (entry is null)
                return NotFound();

            await _temperatureRepository.UpdateTemperatureEntry(id, newEntry);

            return NoContent();
        }


        [APIKeyAuth("admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemperatureEntry(int id)
        {
            TemperatureModel? entry = await _temperatureRepository.GetTemperatureEntry(id);

            if (entry is null)
                return NotFound();

            await _temperatureRepository.DeleteTemperatureEntry(id);

            return NoContent();
        }
    }
}
