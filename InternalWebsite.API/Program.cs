using InternalWebsite.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElsaQuickstarts.Server.DashboardAndServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logBuilder =>
                {
                    logBuilder.ClearProviders();  // Removes all default providers
                    logBuilder.AddConsole();      // Adds Console logger
                    logBuilder.AddDebug();        // Adds Debug logger
                    logBuilder.AddTraceSource("Information, ActivityTracing"); // Adds TraceSource logger
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

}
