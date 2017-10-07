using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using App42.Server.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace App42.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            MigrateDb();
            host.Run();
        }

        private static void MigrateDb()
        {
            using (var db = new App42Context())
            {
                db.Database.Migrate();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .Build();
    }
}
