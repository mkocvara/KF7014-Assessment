using System.ComponentModel.DataAnnotations;

namespace IoTHumidityDataCollector.Models
{
    public class HumidityReading
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [Range(0.0, 100.0,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Percentage { get; set; }

        [Required]
        [Range(-90.0, 90.0,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Latitude { get; set; }

        [Required]
        [Range(-180.0, 180.0,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Longitude { get; set; }
        
        public override string ToString()
        {
            return $"[{Id}]: {Percentage} @ {Timestamp} {Latitude},{Longitude} ";
        }
    }
}
