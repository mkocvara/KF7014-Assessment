using IoTHumidityDataCollector.Connectors;
using IoTHumidityDataCollector.Models;
using Microsoft.AspNetCore.Mvc;

namespace IoTDataCollector.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class HumidityController : ControllerBase
    {
        private readonly ILogger<HumidityController> _logger;
        private readonly HumidityContext Context;

        public HumidityController(ILogger<HumidityController> logger,
            HumidityContext context)
        {
            _logger = logger;
            Context = context;
        }

        [HttpGet(Name = "GetHumidityReadings")]
        [ApiVersion("1.0")]
        [Produces("application/json")]
        public IActionResult Get()
        {
            if (Context.hReadings.Any())
                return Ok(Context.hReadings.ToArray());
            return NotFound();
        }

        [HttpGet("{id}", Name = "GetHumidityReading")]
        [ApiVersion("1.0")]
        [Produces("application/json")]
        public IActionResult Get(int id)
        {
            var results = Context.hReadings.Where(r => r.Id == id)
                                                .ToList().FirstOrDefault();
            if (results != null)
                return Ok(results);
            return NotFound();
        }

        // The method accepts a list to facilitate easier batch submissions for data
        [HttpPost(Name = "CreateHumidityReadings")]
        [ApiVersion("1.0")]
        [Produces("application/json")]
        public IActionResult Create(List<HumidityReading> hReadings)
        {
            Console.WriteLine(hReadings[0].ToString());

            // Check for value errors in the submitted data.
            int index = 0;
            foreach (HumidityReading hr in hReadings)
            {
                if(hr.Timestamp.Equals(new DateTime(0)))
                    return BadRequest($"Index {index}: Timestamp cannot be null");
                else if(hr.Id != 0)
                    return BadRequest($"Index {index}: The 'id' attribute must not be set");
                else if (hr.Timestamp > DateTime.UtcNow)
                    return BadRequest($"Index {index}: The 'timestamp' parameter can not be after the current UTC time.");
                
                index++;
            }

            Context.hReadings.AddRange(hReadings);
            Context.SaveChanges();

            // Create the list of IDs to be returned
            List<int> ids = new List<int>();
            hReadings.ForEach(hr =>
            {
                ids.Add(hr.Id);
            });

            return Ok(ids);
        }

        [HttpPut(Name = "UpdateHumidityReading")]
        [ApiVersion("1.0")]
        [Produces("application/json")]
        public IActionResult Update(HumidityReading hReadingRequest)
        {
            HumidityReading hReading = Context.hReadings.SingleOrDefault(
                                            hr => hr.Id == hReadingRequest.Id);
            if (hReading != null)
            {
                if (hReadingRequest.Timestamp > DateTime.UtcNow)
                {
                    return BadRequest("The 'timestamp' parameter can not be after the current UTC time.");
                }

                Context.hReadings.Entry(hReading).CurrentValues
                                        .SetValues(hReadingRequest);
                Context.SaveChanges();
                return NoContent();
            }
            else return BadRequest();
        }

        [HttpDelete("{id}", Name = "DeleteHumidityReading")]
        [ApiVersion("1.0")]
        [Produces("application/json")]
        public IActionResult Delete(int id)
        {
            HumidityReading hReading = Context.hReadings.SingleOrDefault(hr => hr.Id == id);
            if (hReading != null) { 
                Context.hReadings.Remove(hReading);
                Context.SaveChanges();
                return NoContent();
            }else
                return NotFound();
        }
    }
}