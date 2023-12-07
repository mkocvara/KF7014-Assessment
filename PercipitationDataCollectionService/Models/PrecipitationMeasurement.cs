using System.Data;

namespace PrecipitationService.Db
{
    public class PrecipitationMeasurement
    {
        required public int Id { get; set; } 

        // Location of measurement
        public string? Location { get; set; }

        // Date and time of measurement
        public DateTime? DateTime { get; set; }

        // How much precipitation fell in mm on the measured day
        public float? PrecipitationMm { get; set; }

        // Precipitation in inches
        public float? PrecipitationIn => PrecipitationMm / 25.4f;

        // Percentage of the day with precipitation
        public float? Coverage { get; set; }

        // How much snow fell in cm that day
        public float? Snowfall { get; set; }

        // Depth of snow in cm when measured
        public float? SnowDepth { get; set; }

        // Is there a severe weather risk based on the precipitation?
        public bool SevereRisk { get; set; } = false;

        public void EvaluateRisk()
        {
            // If there is more than 25mm of precipitation in an hour, there is a severe weather risk
            bool torrentialRain = (PrecipitationMm / (24 * (Coverage/100)) > 25.0f);

            // If there is more than 30cm of snowfall in a single day, there is a severe weather risk
            bool heavySnow = Snowfall > 30.0f;    

            SevereRisk = torrentialRain || heavySnow;
        }
    }
}