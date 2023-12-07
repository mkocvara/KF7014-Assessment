﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PrecipitationService.Db;

#nullable disable

namespace PrecipitationService.Migrations
{
    [DbContext(typeof(PrecipitationDb))]
    partial class PrecipitationDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.14");

            modelBuilder.Entity("PrecipitationService.Db.PrecipitationMeasurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float?>("Coverage")
                        .HasColumnType("REAL");

                    b.Property<DateTime?>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<float?>("PrecipitationMm")
                        .HasColumnType("REAL");

                    b.Property<bool>("SevereRisk")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("SnowDepth")
                        .HasColumnType("REAL");

                    b.Property<float?>("Snowfall")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Measurements");
                });
#pragma warning restore 612, 618
        }
    }
}
