using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using App42.Server.Data;

namespace App42.Server.Migrations
{
    [DbContext(typeof(App42Context))]
    partial class App42ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("App42.Server.Data.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int>("Discriminator");

                    b.Property<int>("TripId");

                    b.HasKey("Id");

                    b.HasIndex("TripId");

                    b.ToTable("Events");

                    b.HasDiscriminator<int>("Discriminator");
                });

            modelBuilder.Entity("App42.Server.Data.Trip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("InProgress");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("Trips");
                });

            modelBuilder.Entity("App42.Server.Data.BatteryEvent", b =>
                {
                    b.HasBaseType("App42.Server.Data.Event");

                    b.Property<int>("Charge");

                    b.ToTable("BatteryEvent");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("App42.Server.Data.LocationEvent", b =>
                {
                    b.HasBaseType("App42.Server.Data.Event");

                    b.Property<float>("Accuracy");

                    b.Property<decimal>("Lat");

                    b.Property<decimal>("Lon");

                    b.Property<float>("Speed");

                    b.ToTable("LocationEvent");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("App42.Server.Data.Event", b =>
                {
                    b.HasOne("App42.Server.Data.Trip", "Trip")
                        .WithMany()
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
