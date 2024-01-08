namespace ClientApp.Data
{
    public class PrecipitationMeasurement
    {
        public int Id { get; set; }
        public string? Location { get; set; }
        public DateTime? DateTime { get; set; }
        public float? PrecipitationMm { get; set; }
        public float? Coverage { get; set; }
        public float? Snowfall { get; set; }
        public float? SnowDepth { get; set; }
        public bool SevereRisk { get; set; }

        public PrecipitationMeasurement() { }

        public PrecipitationMeasurement(int id, string location, DateTime dateTime, float precipitationMm, float coverage, float snowfall, float snowDepth, bool severe)
        {
            Id = id;
            Location = location;
            DateTime = dateTime;
            PrecipitationMm = precipitationMm;
            Coverage = coverage;
            Snowfall = snowfall;
            SnowDepth = snowDepth;
            SevereRisk = severe;
        }
    }
}
