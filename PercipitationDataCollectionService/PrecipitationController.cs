using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrecipitationService.Db;

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

        /// <summary>
        /// HTTP GET method for retrieving all measurements.
        /// </summary>
        /// <returns>HTTP response: 404 if resource can't be found; 200 with a List<> of all measurements otherwise.</returns>
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

        /// <summary>
        /// HTTP GET method for retrieving a single measurement.
        /// </summary>
        /// <param name="id">Id of the measurement to retrieve.</param>
        /// <returns>HTTP response: 404 if resource can't be found; 200 with the requested measurement otherwise.</returns>
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

        /// <summary>
        /// HTTP GET method for retrieving measurement with severe weather risk.
        /// </summary>
        /// <returns>HTTP response: 404 if resource can't be found; 200 with a List<> of measurement where SevereRisk is true otherwise.</returns>
        [HttpGet("severe")]
        public async Task<ActionResult<IEnumerable<PrecipitationMeasurementDTO>>> GetSevereMeasurements()
        {
            if (_dbContext.Measurements == null)
            {
                return NotFound();
            }

            return await _dbContext.Measurements
                .Where(t => t.SevereRisk)
                .Select(t => new PrecipitationMeasurementDTO(t))
                .ToListAsync();
        }

        /// <summary>
        /// HTTP GET method for retrieving measurement from a specific location.
        /// </summary>
        /// <param name="location">Location of the measurements to retreive.</param>
        /// <returns>HTTP response: 404 if resource can't be found; 200 with a List<> of measurement where Location matches the supplied location.</returns>
        [HttpGet("location/{location}")]
        public async Task<ActionResult<IEnumerable<PrecipitationMeasurementDTO>>> GetMeasurementsByLocation(string location)
        {
            if (_dbContext.Measurements == null)
            {
                return NotFound();
            }

            return await _dbContext.Measurements
                .Where(t => location.Equals(t.Location))  
                .Select(t => new PrecipitationMeasurementDTO(t))
                .ToListAsync();
        }

        /// <summary>
        /// HTTP POST method for creating a new measurement and adding it to the database.
        /// </summary>
        /// <param name="measurementDto">A measurement DTO object containing the data for creating the new measurement.</param>
        /// <returns>HTTP response: 200 with the newly created measurement's data, if it was successfully created.</returns>
        [HttpPost]
        public async Task<ActionResult<PrecipitationMeasurementDTO>> PostMeasurement(PrecipitationMeasurementDTO measurementDto)
        {
            // Console.WriteLine($"POST: API invoked at {DateTime.Now}..."); // for testing            

            if (_dbContext.Measurements == null)
            {
                return Problem("Entity set 'PrecipitationDb.Measurements' is null.");
            }

            if(measurementDto == null)
            {
                return BadRequest();
            }

            PrecipitationMeasurement measurement = measurementDto.MakePrecipitationMeasurement();
            AssessRisk(measurement);

            lock (_dbContext.Measurements)
            {
                _dbContext.Measurements.Add(measurement);
                _dbContext.SaveChangesAsync();
                // Console.WriteLine($"POST: Adding new measurement with id {measurement.Id} to the database..."); // for testing            
            }
            measurementDto.Id = measurement.Id;

            return CreatedAtAction(nameof(GetMeasurement), new { id = measurementDto.Id }, measurementDto);
        }

        /// <summary>
        /// HTTP PUT method for updating a measurement.
        /// </summary>
        /// <param name="id">Id of measurement to update.</param>
        /// <param name="measurementDto">A measurement DTO object containing the new data.</param>
        /// <returns>HTTP response: 400 if measurement with the provided id does not exist; 204 otherwise.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasurement(int id, PrecipitationMeasurementDTO measurementDto)
        {
            if (measurementDto == null || !MeasurementExists(id))
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

        /// <summary>
        /// HTTP DELETE method for deleting a measurement from the database.
        /// </summary>
        /// <param name="id">The ID of the measurement to delete.</param>
        /// <returns>HTTP response: 404 if measurement with the provided id cannot be found; 204 otherwise.</returns>
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

        // Checks if a measurement with the given ID exists in the database.
        private bool MeasurementExists(int id)
        {
            return (_dbContext.Measurements?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // Checks if the given measurement represents a weather risk, and prints a warning if it does.
        private void AssessRisk(PrecipitationMeasurement measurement)
        {
            if (measurement.SevereRisk)
            {
                Console.WriteLine("=================================");
                Console.WriteLine("= SEVERE WEATHER RISK DETECTED! =");
                Console.WriteLine("=================================");
            }
        }
    }
}