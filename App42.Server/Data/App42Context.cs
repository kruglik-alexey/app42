using System;
using System.IO;
using System.Reflection;
using App42.Common;
using Microsoft.EntityFrameworkCore;

namespace App42.Server.Data
{
    public class App42Context : DbContext
    {
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "app42.sqlite");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasDiscriminator<int>("Discriminator")
                .HasValue<LocationEvent>(LocationEvent.Descriminator)
                .HasValue<BatteryEvent>(BatteryEvent.Descriminator);
        }
    }
}