namespace ClientApp.Data;

public class WeatherMeasurement
{
    public string? Location { get; set; }
    public DateTime? DateTime { get; set; }

    // Precipitation
    public float? PrecipitationMm { get; set; }
    public float? Coverage { get; set; }
    public float? Snowfall { get; set; }
    public float? SnowDepth { get; set; }
    public bool PrecipitationSevereRisk { get; set; }

    // Temperature
    public float? TemperatureC { get; set; }

    // Humidity
    public double? Humidity { get; set; }


    public WeatherMeasurement() { }

    public WeatherMeasurement(PrecipitationMeasurement precip, TemperatureMeasurement temp, HumidityMeasurement humidity) 
    {
        bool badData = false;
        if (precip.Location != temp.Location || temp.Location != humidity.Location)
        {
            Console.WriteLine("Error: Measurements must be from the same location.");
            badData = true;
        }

            if (!precip.DateTime.HasValue || !temp.DateTime.HasValue || !humidity.Timestamp.HasValue ||
                precip.DateTime.Value.Date.CompareTo(temp.DateTime.Value.Date) != 0 || temp.DateTime.Value.Date.CompareTo(humidity.Timestamp.Value.Date) != 0)
            {
                Console.WriteLine("Error: Measurements must be from the same day.");
                badData = true;
            }

        if (badData)
            return; // this will result in an object with all null fields

        Location = precip.Location;
        DateTime = precip.DateTime.HasValue ? precip.DateTime.Value.Date : null;

        PrecipitationMm = precip.PrecipitationMm;
        Coverage = precip.Coverage;
        Snowfall = precip.Snowfall;
        SnowDepth = precip.SnowDepth;
        PrecipitationSevereRisk = precip.SevereRisk;

        TemperatureC = temp.Temperature;

            Humidity = humidity.Percentage;
        }

    public WeatherMeasurement(string? location, DateTime? dateTime, 
                                float? precipitationMm, float? coverage, float? snowfall, float? snowDepth, bool precipSevere,
                                float? tempC, 
                                double? humidity)
    {
        Location = location;
        DateTime = dateTime;

        PrecipitationMm = precipitationMm;
        Coverage = coverage;
        Snowfall = snowfall;
        SnowDepth = snowDepth;
        PrecipitationSevereRisk = precipSevere;

        TemperatureC = tempC;

        Humidity = humidity;
    }

    public WeatherMeasurement CopyDataFromPrecipitationMeasurement(PrecipitationMeasurement precip)
    {
        if (!CheckLocationAndDate(precip.Location, precip.DateTime))
            return this;

        PrecipitationMm = precip.PrecipitationMm;
        Coverage = precip.Coverage;
        Snowfall = precip.Snowfall;
        SnowDepth = precip.SnowDepth;
        PrecipitationSevereRisk = precip.SevereRisk;

        return this;
    }

    public WeatherMeasurement CopyDataFromTemperatureMeasurement(TemperatureMeasurement temp)
    {
        if (!CheckLocationAndDate(temp.Location, temp.DateTime))
            return this;

        TemperatureC = temp.Temperature;

        return this;
    }

        public WeatherMeasurement CopyDataFromHumidityMeasurement(HumidityMeasurement humidity)
        {
            if (!CheckLocationAndDate(humidity.Location, humidity.Timestamp))
                return this;

            Humidity = humidity.Percentage;

        return this;
    }

    private bool CheckLocationAndDate(string? newLoc, DateTime? newDateTime)
    {

        if (Location is null)
        {
            Location = newLoc;
        }
        else if (Location != newLoc)
        {
            Console.WriteLine("Warning: Measurements must be from the same location.");
            return false;
        }

        if (DateTime is null)
        {
            DateTime = newDateTime;
        }
        else if (!newDateTime.HasValue || DateTime.Value.Date.CompareTo(newDateTime.Value.Date) != 0)
        {
            Console.WriteLine("Warning: Measurements must be from the same day.");
            return false;
        }

        return true;
    }

    public bool HasSameData(WeatherMeasurement wm)
    {
        return
            Location == wm.Location &&
            DateTime == wm.DateTime &&
            PrecipitationMm == wm.PrecipitationMm &&
            Coverage == wm.Coverage &&
            Snowfall == wm.Snowfall &&
            SnowDepth == wm.SnowDepth &&
            PrecipitationSevereRisk == wm.PrecipitationSevereRisk &&

            TemperatureC == wm.TemperatureC &&

            Humidity == wm.Humidity;
    }
}
