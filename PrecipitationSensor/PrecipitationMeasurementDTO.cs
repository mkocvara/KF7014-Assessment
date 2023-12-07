namespace PrecipitationSensor
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

        public static PrecipitationMeasurementDTO GetTestMeasurement()
        {
            Func<double, double> truncTwo = (double d) => Math.Truncate(d * 100.0) / 100.0;

            // random number generator
            System.Random random = new System.Random();
            float randPrecipitationMm = (float)truncTwo(random.NextDouble() % 125.0);
            float randCoverage = (float)truncTwo(random.NextDouble() % 100.0);
            float randSnowfall = (random.NextInt64() % 8 == 0) ? (float)truncTwo(random.NextDouble() % 1.0) : 0.0f;
            testSnowDepth =+ Math.Max(0, randSnowfall - (float)truncTwo(random.NextDouble() % 0.2));

            randPrecipitationMm = (float)(Math.Truncate((double)randPrecipitationMm * 100.0) / 100.0);
            return new PrecipitationMeasurementDTO(testCounter, "TestLocation" + testCounter++, System.DateTime.Now, randPrecipitationMm, randCoverage, randSnowfall, testSnowDepth);
        }
    }
}