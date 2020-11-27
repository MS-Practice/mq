using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace MQ.CAP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logger =>
                {
                    logger.ClearProviders();
                    logger.SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureAppConfiguration((host, cfg) =>
                {
                    cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    if (!string.IsNullOrEmpty(host.HostingEnvironment.EnvironmentName))
                        cfg.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseNLog();
    }
}
