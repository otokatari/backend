using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OtokatariBackend.Model.DependencyInjection.Databases;
using System;

namespace OtokatariBackend.Persistence.MongoDB.DependencyInjection
{
    public static class MongoClientContext
    {
        public static IServiceCollection AddMongoDB(this IServiceCollection services,Action<MongoClientConfiguration> configure)
        {
            var config = new MongoClientConfiguration();
            configure(config);
            return services.AddSingleton(_ => new MongoClient(config.ConnectionString).GetDatabase(config.Database));
        }

        public static IServiceCollection AddMongoDB(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider()
                                 .GetRequiredService<IOptions<MongoClientConfiguration>>()
                                 .Value;
            return services.AddSingleton(_ => new MongoClient(config.ConnectionString).GetDatabase(config.Database));
        }
    }
}
