using System.Xml.Linq;

namespace PrecipitationService.Db
{
    public class PrecipitationMeasurementDTO
    {
        public int Id { get; set; } 
        public string? Location { get; set; }
        public DateTime? DateTime { get; set; }
        public float? PrecipitationMm { get; set; }
        public float? Coverage { get; set; }
        public float? Snowfall { get; set; }
        public float? SnowDepth { get; set; }

        public PrecipitationMeasurementDTO() { }
        public PrecipitationMeasurementDTO(PrecipitationMeasurement m)
        {
            Id = m.Id;
            Location = m.Location;
            DateTime = m.DateTime;
            PrecipitationMm = (m.PrecipitationMm is null && m.PrecipitationIn is not null) ? (m.PrecipitationIn / 25.4f) : m.PrecipitationMm;
            Coverage = m.Coverage;
            Snowfall = m.Snowfall;
            SnowDepth = m.SnowDepth;
        }
            
        public PrecipitationMeasurement MakePrecipitationMeasurement()
        {
            PrecipitationMeasurement m = new()
            {
                Id = this.Id,
                Location = this.Location,
                DateTime = this.DateTime,
                PrecipitationMm = this.PrecipitationMm,
                Coverage = this.Coverage,
                Snowfall = this.Snowfall,
                SnowDepth = this.SnowDepth
            };

            m.EvaluateRisk();
            return m;
        }
    }
}