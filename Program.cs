using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace OtokatariBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                     var env = hostingContext.HostingEnvironment;
                     var Home = args.Length > 0 ? "Home":"";
                     var configJson = $"appsettings.{env.EnvironmentName}.{Home}.json";
                     System.Console.WriteLine("Configuration Loaded: "+configJson);
                     config.AddJsonFile(configJson, optional: true, reloadOnChange: true);
                     config.AddEnvironmentVariables();
                     config.AddUserSecrets<Startup>();
                });
    }
}
