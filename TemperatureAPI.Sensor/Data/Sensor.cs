﻿using System.Diagnostics;
using System.Timers;
using System.Text.Json;
using TemperatureAPI.Sensor.Services;
using Timer = System.Timers.Timer;
using System.Text;

namespace TemperatureAPI.Sensor.Data
{
    public class Sensor
    {
        

        public int Id { get; set; }
        public string Description { get; set; } = "";
        public int IntervalInMinutes { get; set; }
        private readonly Random _rng = new();
        public Timer? _timer;
        private readonly HttpClient _client = new();
        

        public Sensor(int id, string description, int intervalInMinutes = -1)
        {
            //int delay = 10; //seconds
            //Console.WriteLine($"{id} @ {description} sleeping {delay}s");
            //Thread.Sleep(delay * 1000);
            Id = id;
            Description = description;
            if (intervalInMinutes < 0)
            {
                SetupInterval();
            } else
            {
                IntervalInMinutes = intervalInMinutes;
            }
            updateTimer();
            // adds the APIKey for the sensor rights
            _client.DefaultRequestHeaders.Add("APIKey", "85f82393-59ca-485e-99f3-01d9915bd195");
            SendTemperature();
        }



        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            SendTemperature();
        }

        public void SetupInterval()
        {
            int choice = _rng.Next(0, 3);
            IntervalInMinutes = choice switch
            {
                1 => 1 * 60,
                2 => 3 * 60,
                _ => 30,
            };
        }

        public async Task SendTemperature()
        {
            Dictionary<String, Object> temperatureObject = new()
            {
                { "SensorId", Id },
                { "SensorDescription", Description},
                { "Temperature", GetCurrentTemperature() },
                { "Date", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff") },
            };
            StringContent content = new(
                JsonSerializer.Serialize(temperatureObject),
                Encoding.UTF8, "application/json");

            try
            {
                String url = (Environment.GetEnvironmentVariable("TARGET_URL") ?? "https://localhost:7081") + "/Temperature";
                HttpResponseMessage response = await _client.PostAsync(url, content);
                Console.WriteLine($"{url}: status {response.StatusCode} with message:");
                Console.WriteLine(response.Content.ToString());
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // Returns the temperature emulated by the sensor
        public int GetCurrentTemperature() => _rng.Next(-15, 40);

        public void CopyFrom(Sensor sensor)
        {
            Id = sensor.Id;
            Description = sensor.Description;
        }

        public void updateTimer()
        {
            _timer?.Stop();
            _timer = new(IntervalInMinutes * 60 * 1000);
            _timer.AutoReset = true;
            _timer.Elapsed += OnTimedEvent;
            _timer.Enabled = true;
        }
    }
}