﻿// <auto-generated />
using System;
using HomeStation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HomeStation.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AirDbContext))]
    partial class AirDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HomeStation.Domain.Common.Entities.Climate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<double>("Humidity")
                        .HasColumnType("float");

                    b.Property<double>("Pressure")
                        .HasColumnType("float");

                    b.Property<double>("Temperature")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("Climate");
                });

            modelBuilder.Entity("HomeStation.Domain.Common.Entities.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsKnown")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("HomeStation.Domain.Common.Entities.Quality", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int>("Pm10")
                        .HasColumnType("int");

                    b.Property<int>("Pm1_0")
                        .HasColumnType("int");

                    b.Property<int>("Pm2_5")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("AirQuality");
                });

            modelBuilder.Entity("HomeStation.Domain.Common.Entities.Climate", b =>
                {
                    b.HasOne("HomeStation.Domain.Common.Entities.Device", "Device")
                        .WithMany("Climate")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("HomeStation.Domain.Common.Entities.Reading", "Reading", b1 =>
                        {
                            b1.Property<Guid>("ClimateId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTimeOffset>("Date")
                                .HasColumnType("datetimeoffset");

                            b1.HasKey("ClimateId");

                            b1.ToTable("Climate");

                            b1.WithOwner()
                                .HasForeignKey("ClimateId");
                        });

                    b.Navigation("Device");

                    b.Navigation("Reading")
                        .IsRequired();
                });

            modelBuilder.Entity("HomeStation.Domain.Common.Entities.Quality", b =>
                {
                    b.HasOne("HomeStation.Domain.Common.Entities.Device", "Device")
                        .WithMany("AirQuality")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("HomeStation.Domain.Common.Entities.Reading", "Reading", b1 =>
                        {
                            b1.Property<Guid>("QualityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTimeOffset>("Date")
                                .HasColumnType("datetimeoffset");

                            b1.HasKey("QualityId");

                            b1.ToTable("AirQuality");

                            b1.WithOwner()
                                .HasForeignKey("QualityId");
                        });

                    b.Navigation("Device");

                    b.Navigation("Reading")
                        .IsRequired();
                });

            modelBuilder.Entity("HomeStation.Domain.Common.Entities.Device", b =>
                {
                    b.Navigation("AirQuality");

                    b.Navigation("Climate");
                });
#pragma warning restore 612, 618
        }
    }
}
