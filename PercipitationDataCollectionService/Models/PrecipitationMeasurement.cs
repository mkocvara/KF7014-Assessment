namespace PrecipitationService.Db
{
    public class PrecipitationMeasurement
    {
        required public int Id { get; set; } 

        // Location of measurement
        public string? Location { get; set; }

        // Date and time of measurement
        public DateTime? DateTime { get; set; }

        // How much precipitation fell in mm that period
        public float? PrecipitationMm { get; set; }

        // Precipitation in inches
        public float? PrecipitationIn => PrecipitationMm / 25.4f;

        // Percentage of measured period with precipitation
        public float? Coverage { get; set; }

        // How much snow fell in mm that period
        public float? Snowfall { get; set; }

        // Depth of snow in mm when measured
        public float? SnowDepth { get; set; }

        // Is there a severe weather risk based on the precipitation?
        public bool? SevereRisk { get; set; }
    }
}