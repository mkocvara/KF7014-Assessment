using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

namespace PrecipitationSensors.Pages
{
    public class IndexModel : PageModel
    {
        public readonly SensorService SensorService;

        public IndexModel(SensorService sensorService)
        {
            SensorService = sensorService;
        }

        public void OnGet()
        {
            // Console.WriteLine("OnGet() called!");
        }

        // Gets the partial view with details of the sensors
        public PartialViewResult OnGetSensorsDetails()
        {
            // Console.WriteLine("OnGetSensorsDetails() called!");
            return Partial("_SensorsDetails", SensorService.Sensors);
        }
    }
}