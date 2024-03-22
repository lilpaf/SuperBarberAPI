using Serilog;
using Serilog.Core;
using System.Text;

namespace NotificationService.Extensions
{
    public static class HostApplicationBuilderExtensions
    {
        public static IHostApplicationBuilder UseSerilog(this IHostApplicationBuilder builder)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Logger logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            return builder;
        }
    }
}
