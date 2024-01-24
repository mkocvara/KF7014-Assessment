using System.ComponentModel.DataAnnotations;

namespace IoTHumidityDataCollector.Models
{
    public class HumidityReading
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Timestamp must be set")]
        public DateTime? Timestamp { get; set; }

        [Required(ErrorMessage = "Percentage must be set")]
        [Range(0.0, 100.0,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double? Percentage { get; set; }

        [Required(ErrorMessage = "Latitude must be set")]
        [Range(-90.0, 90.0,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double? Latitude { get; set; }

        [Required(ErrorMessage = "Longitude must be set")]
        [Range(-180.0, 180.0,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double? Longitude { get; set; }

        [Required]
        public string? Location { get; set; }

        public override string ToString()
        {
            return $"[{Id}]: {Percentage} @ {Timestamp} {Latitude},{Longitude} ";
        }
    }
}
