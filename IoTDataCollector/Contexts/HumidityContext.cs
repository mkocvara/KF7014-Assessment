﻿using EasyNetQ;
using IoTHumidityDataCollector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.Metrics;
using System.Security;

namespace IoTHumidityDataCollector.Connectors
{
    public class HumidityContext : DbContext
    {
        public DbSet<HumidityReading> hReadings { get; set; }

        private int MaximumHumidityPercentageValue;
        private int MinimumHumidityPercentageValue;

        private readonly IBus _eventBus;

        public HumidityContext(DbContextOptions<HumidityContext> options, IBus eventBus) : base(options)
        {
            // Add event subscriptions to check for critical values
            // when items are inserted or updated
            ChangeTracker.StateChanged += CheckForCriticalValues;
            ChangeTracker.Tracked += CheckForCriticalValues;

            _eventBus = eventBus;
        }

        // Notification Function. Checks for critical values during created or modified events
        private void CheckForCriticalValues(object sender, EntityEntryEventArgs e)
        {
            if (e.Entry.Entity is HumidityReading hr)
            {
                if (e.Entry.State != EntityState.Deleted)
                {
                    bool isSevere = false;

                    if (hr.Percentage >= MaximumHumidityPercentageValue)
                    {
                        Console.WriteLine($"[ALERT] - ({hr.Id}) Maximum limit reached. Current value: {hr.Percentage} @ {hr.Timestamp} {hr.Latitude}, {hr.Longitude}: Maximum {MaximumHumidityPercentageValue}");
                        isSevere = true;
                    }
                    else if (hr.Percentage <= MinimumHumidityPercentageValue)
                    {
                        Console.WriteLine($"[ALERT] - ({hr.Id}) Minimum limit reached. Current value: {hr.Percentage} @ {hr.Timestamp} {hr.Latitude}, {hr.Longitude}: Minimum {MinimumHumidityPercentageValue}");
                        isSevere = true;
                    }

                    // Publish the new measurement's id as a message to the event bus
                    try
                    {
                        if (isSevere)
                            _eventBus.PubSub.Publish(hr.Id, "Humidity");
                    }
                    catch
                    {
                        Console.WriteLine($"Failed to publish message to event bus. Very possibly, RabbitMQ is not running.");
                    }
                }
            }
        }

        private string retrieveEnvironmentVariable(string envVar)
        {
            string value = null;
            try
            {
                value = Environment.GetEnvironmentVariable(envVar);
                if (value == null)
                {
                    Console.WriteLine($"Environment variable {envVar} is not set.");
                }
                return value;
            }
            catch (SecurityException)
            {
                Console.WriteLine($"Cannot access the {envVar} environment variable");
            }
            return value;
        }

        private int retrieveNumericalEnvironmentVariable(string envVar)
        {
            try
            {
                string value = retrieveEnvironmentVariable(envVar);
                if (value == null)
                    Environment.Exit(1);
                return Math.Clamp(Int32.Parse(value), 0, 100);
            }
            catch (Exception e)
            {
                if (e is FormatException)
                    Console.WriteLine($"Invalid value for the {envVar} environment variable: contains non-numerical characters.");
                else if (e is OverflowException)
                    Console.WriteLine($"Invalid value for the {envVar} environment variable: is out of the limits of Int32. ({Int32.MinValue}, {Int32.MaxValue})");
                return -1;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            MinimumHumidityPercentageValue = retrieveNumericalEnvironmentVariable("HUMIDITY_MIN");
            if(MinimumHumidityPercentageValue == -1) {
                Environment.Exit(0);
            }

            MaximumHumidityPercentageValue = retrieveNumericalEnvironmentVariable("HUMIDITY_MAX");
            if (MinimumHumidityPercentageValue == -1)
            {
                Environment.Exit(0);
            }

            string connectionString = retrieveEnvironmentVariable("MYSQL_URI");

            if (connectionString == null)
                Environment.Exit(0);
            
            optionsBuilder.UseMySQL(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set up an autoincremented primary key
            // for HumidityReading
            modelBuilder.Entity<HumidityReading>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();
            });        
        }
    }
}
