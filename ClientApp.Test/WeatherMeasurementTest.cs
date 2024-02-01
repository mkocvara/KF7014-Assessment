using ClientApp.Data;

namespace ClientApp.Test
{
    [TestClass]
    public class WeatherMeasurementTest
    {
        [TestMethod]
        public bool CopyDataFromPrecipitationMeasurement_ShouldCopy()
        {
            WeatherMeasurement target = new();
            PrecipitationMeasurement precip = new();
            target.Location        = precip.Location        = "Test";
            target.DateTime        = precip.DateTime        = new DateTime(2021, 1, 1);
            target.PrecipitationMm = precip.PrecipitationMm = 1.0f;

            WeatherMeasurement test = new();
            test.CopyDataFromPrecipitationMeasurement(precip);

            return test.HasSameData(target);
        }

        [TestMethod]
        public bool CopyDataFromTemperatureMeasurement_ShouldCopy()
        {
            WeatherMeasurement target = new();
            TemperatureMeasurement temp = new();
            target.Location = temp.Location = "Test";
            target.DateTime = temp.DateTime = new DateTime(2021, 1, 1);
            target.TemperatureC = temp.Temperature = 1;

            WeatherMeasurement test = new();
            test.CopyDataFromTemperatureMeasurement(temp);

            return test.HasSameData(target);
        }

        [TestMethod]
        public bool CopyDataFromHumidityMeasurement_ShouldCopy()
        {
            WeatherMeasurement target = new();
            HumidityMeasurement hum = new();
            target.Location = hum.Location = "Test";
            target.DateTime = hum.Timestamp = new DateTime(2021, 1, 1);
            target.Humidity = hum.Percentage = 1.0f;

            WeatherMeasurement test = new();
            test.CopyDataFromHumidityMeasurement(hum);

            return test.HasSameData(target);
        }

        // Should not copy due to differing location and/or date
        [TestMethod]
        public bool CopyDataFromPrecipitationMeasurement_ShouldNotCopy()
        {
            WeatherMeasurement target1 = new();
            WeatherMeasurement target2 = new();
            PrecipitationMeasurement precip = new();
            target1.Location = precip.Location = "Test";
            target2.Location = "Test2";
            target1.DateTime = new DateTime(2021, 1, 2);
            target2.DateTime = precip.DateTime = new DateTime(2021, 1, 1);
            target2.PrecipitationMm = target1.PrecipitationMm = precip.PrecipitationMm = 1.0f;

            WeatherMeasurement test = new();
            test.CopyDataFromPrecipitationMeasurement(precip);
            return !test.HasSameData(target1) && !test.HasSameData(target2);
        }

        [TestMethod]
        public bool CopyDataFromTemperatureMeasurement_ShouldNotCopy()
        {
            WeatherMeasurement target1 = new();
            WeatherMeasurement target2 = new();
            TemperatureMeasurement temp = new();
            target1.Location = temp.Location = "Test";
            target2.Location = "Test2";
            target1.DateTime = new DateTime(2021, 1, 2);
            target2.DateTime = temp.DateTime = new DateTime(2021, 1, 1);
            target2.TemperatureC = target1.TemperatureC = temp.Temperature = 1;

            WeatherMeasurement test = new();
            test.CopyDataFromTemperatureMeasurement(temp);
            return !test.HasSameData(target1) && !test.HasSameData(target2);
        }

        [TestMethod]
        public bool CopyDataFromHumidityMeasurement_ShouldNotCopy()
        {
            WeatherMeasurement target1 = new();
            WeatherMeasurement target2 = new();
            HumidityMeasurement hum = new();
            target1.Location = hum.Location = "Test";
            target2.Location = "Test2";
            target1.DateTime = new DateTime(2021, 1, 2);
            target2.DateTime = hum.Timestamp = new DateTime(2021, 1, 1);
            target2.Humidity = target1.Humidity = hum.Percentage = 1.0f;

            WeatherMeasurement test = new();
            test.CopyDataFromHumidityMeasurement(hum);
            return !test.HasSameData(target1) && !test.HasSameData(target2);
        }
    }
}