using System.ComponentModel.DataAnnotations;

namespace ClientApp.Data
{
    public class HumidityMeasurement
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime? Timestamp { get; set; }

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

        public string? Location { get; set; }

        public HumidityMeasurement() { }

        public HumidityMeasurement(int id, DateTime timestamp, double percentage, 
                                    double latitude, double longitude)
        {
            Id = id;
            Timestamp = timestamp;
            Percentage = percentage;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
