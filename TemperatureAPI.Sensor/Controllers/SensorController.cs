using Microsoft.AspNetCore.Mvc;
using TemperatureAPI.Sensor.Models;
using TemperatureAPI.Temperature.Filters;

namespace TemperatureAPI.Sensor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly ISensorRepository _sensorRepository;
        public SensorController(ISensorRepository sensorRepository)
        {
            _sensorRepository = sensorRepository;
        }

        [HttpGet]
        public ActionResult<List<SensorModel>> GetAllSensors()
        {
            return _sensorRepository.GetAllSensors();
        }

        [APIKeyAuth("admin")]
        [HttpGet("{id}")]
        public ActionResult<SensorModel> GetSensor(int id)
        {
            SensorModel? model = _sensorRepository.GetSensor(id);
            if (model == null)
            {
                return NotFound();
            }
            return model;
        }

        [APIKeyAuth("admin")]
        [HttpPost]
        public ActionResult<SensorModel> CreateSensor(SensorModel model)
        {
            SensorModel createdModel = _sensorRepository.AddSensor(model);

            return CreatedAtAction(
                nameof(GetSensor),
                new { id = createdModel.Id },
                createdModel);
        }

        [APIKeyAuth("admin")]
        [HttpPut]
        public IActionResult UpdateSensor(int id, SensorModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            SensorModel? newSensor = _sensorRepository.UpdateSensor(id, model);

            if (newSensor is null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [APIKeyAuth("admin")]
        [HttpDelete]
        public IActionResult DeleteSensor(int id)
        {
            SensorModel? sensor = _sensorRepository.GetSensor(id);

            if (sensor is null)
                return NotFound();

            _sensorRepository.DeleteSensor(id);

            return NoContent();
        }

    }
}
