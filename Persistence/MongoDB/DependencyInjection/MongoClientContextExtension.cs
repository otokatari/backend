using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OtokatariBackend.Model.DependencyInjection.Databases;
using System;

namespace OtokatariBackend.Persistence.MongoDB.DependencyInjection
{
    public static class MongoClientContextExtension
    {
        public static IServiceCollection AddMongoDB(this IServiceCollection services,Action<MongoClientConfiguration> configure)
        {
            var config = new MongoClientConfiguration();
            var client = new MongoClient(config.ConnectionString);
            return services.AddSingleton(_ => client).AddSingleton(client.GetDatabase(config.Database));
        }

        public static IServiceCollection AddMongoDB(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider()
                                 .GetRequiredService<IOptions<MongoClientConfiguration>>()
                                 .Value;
            var client = new MongoClient(config.ConnectionString);
            return services.AddSingleton(_ => client).AddSingleton(client.GetDatabase(config.Database));
        }
    }

}
