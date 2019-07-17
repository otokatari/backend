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
                     config.AddJsonFile(@"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                     config.AddEnvironmentVariables();
                     config.AddUserSecrets<Startup>();
                });
    }
}
