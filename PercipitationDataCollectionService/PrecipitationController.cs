using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrecipitationService.Db;
using System.Diagnostics.Metrics;

namespace PercipitationService
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrecipitationController : ControllerBase
    {
        private readonly PrecipitationDb _dbContext;

        public PrecipitationController(PrecipitationDb dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Precipitation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrecipitationMeasurementDTO>>> GetAllMeasurements()
        {
            if (_dbContext.Measurements == null)
            {
                return NotFound();
            }

            return await _dbContext.Measurements.Select(t => new PrecipitationMeasurementDTO(t))
                .ToListAsync();
        }

        // GET: api/Precipitation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrecipitationMeasurementDTO>> GetMeasurement(int id)
        {
            if (_dbContext.Measurements == null)
            {
                return NotFound();
            }

            PrecipitationMeasurement? measurement = await _dbContext.Measurements.FindAsync(id);

            if (measurement == null)
            {
                return NotFound();
            }

            return new PrecipitationMeasurementDTO(measurement);
        }

        // PUT: api/Precipitation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasurement(int id, PrecipitationMeasurementDTO measurementDto)
        {
            if (!MeasurementExists(id))
            {
                return BadRequest();
            }

            PrecipitationMeasurement measurement = measurementDto.MakePrecipitationMeasurement();
            measurement.Id = id;

            _dbContext.Entry(measurement).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            AssessRisk(measurement);

            return NoContent();
        }

        // POST: api/Precipitation
        [HttpPost]
        public async Task<ActionResult<PrecipitationMeasurementDTO>> PostMeasurement(PrecipitationMeasurementDTO measurementDto)
        {
            if (_dbContext.Measurements == null)
            {
                return Problem("Entity set 'PrecipitationDb.Measurements' is null.");
            }

            PrecipitationMeasurement measurement = measurementDto.MakePrecipitationMeasurement();
            measurement.Id = _dbContext.Measurements.Count() + 1;

            _dbContext.Measurements.Add(measurement);
            await _dbContext.SaveChangesAsync();

            AssessRisk(measurement);

            measurementDto.Id = measurement.Id;
            return CreatedAtAction(nameof(GetMeasurement), new { id = measurementDto.Id }, measurementDto);
        }

        // DELETE: api/Precipitation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeasurement(int id)
        {
            if (_dbContext.Measurements == null)
            {
                return NotFound();
            }

            PrecipitationMeasurement? measurement = await _dbContext.Measurements.FindAsync(id);

            if (measurement == null)
            {
                return NotFound();
            }

            _dbContext.Measurements.Remove(measurement);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool MeasurementExists(int id)
        {
            return (_dbContext.Measurements?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private void AssessRisk(PrecipitationMeasurement measurement)
        {
            if (measurement.SevereRisk)
                Console.WriteLine("===SEVERE WEATHER RISK DETECTED!==="); // TODO consider something more involved
        }
    }
}