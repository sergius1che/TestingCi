using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace NetCoreConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile($"appsettings.json");
            var conf = builder.Build();

            var slogger = new LoggerConfiguration()
                .ReadFrom.ConfigurationSection(conf.GetSection("Serilog"))
                .CreateLogger();

            var _logger = new LoggerFactory()
                .AddSerilog(slogger)
                //.AddFile(conf.GetSection("Serilog"))
                .CreateLogger("Serilog");

            _logger.LogTrace("This is Trace");
            _logger.LogDebug("This is Debug");
            _logger.LogInformation("This is Information");
            _logger.LogWarning("This is Warning");
            _logger.LogError("This is Error");
            _logger.LogCritical("This is Critical");

            Console.ReadKey();
        }
    }
}
