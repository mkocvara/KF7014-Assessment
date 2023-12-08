namespace PrecipitationSensors
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

        private static int testCounter = 1;
        private static float testSnowDepth = 0.0f;

        public PrecipitationMeasurementDTO() { }

        public PrecipitationMeasurementDTO(int id, string location, DateTime dateTime, float precipitationMm, float coverage, float snowfall, float snowDepth)
        {
            Id = id;
            Location = location;
            DateTime = dateTime;
            PrecipitationMm = precipitationMm;
            Coverage = coverage;
            Snowfall = snowfall;
            SnowDepth = snowDepth;
        }

        // Creates a measurement with random values for testing purposes
        public static PrecipitationMeasurementDTO GetTestMeasurement()
        {
            Func<double, double> truncTwo = (double d) => Math.Truncate(d * 100.0) / 100.0;

            // random number generator
            System.Random random = new System.Random();
            float randPrecipitationMm = (float)truncTwo(Math.Min(random.NextDouble(), random.NextDouble()) % 125.0);
            float randCoverage = (float)truncTwo(Math.Min(random.NextDouble(), random.NextDouble()) % 100.0);
            float randSnowfall = (random.NextInt64() % 8 == 0) ? (float)truncTwo(Math.Min(random.NextDouble(), random.NextDouble()) % 1.0) : 0.0f;
            testSnowDepth = Math.Max(0, testSnowDepth + randSnowfall - (float)truncTwo(Math.Min(random.NextDouble(), random.NextDouble()) % 1.7f));

            // torrential rain
            if (testCounter == 5)
            {
                randPrecipitationMm = 70.0f;
                randCoverage = 10.0f;
                randSnowfall = 0.0f;
                testSnowDepth = 0.0f;
            }

            // heavy snow
            else if (testCounter == 10)
            {
                randPrecipitationMm = 80.0f;
                randCoverage = 75.0f;
                randSnowfall = 35.0f;
                testSnowDepth = 45.0f;
            }

            return new PrecipitationMeasurementDTO(0, "TestLocation" + testCounter++, System.DateTime.Now, randPrecipitationMm, randCoverage, randSnowfall, testSnowDepth);
        }
    }
}