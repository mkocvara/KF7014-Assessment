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

        public PartialViewResult OnGetNextMeasurement()
        {
            // Console.WriteLine("OnGetNextMeasurement() called!");

            return Partial("_MeasurementDetails", SensorService.Sensors);
        }
    }
}