﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OtokatariBackend.Services
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddAllServices<TServ>(this IServiceCollection services,bool IsSingleton = false)
        {
            AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assm => assm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(TServ))))
                        .Where(x => x.IsClass)
                        .ForEachService(x => IsSingleton ? services.AddSingleton(x) : services.AddScoped(x));
            return services;
        }

        public static IEnumerable<T> ForEachService<T>(this IEnumerable<T> serv, Func<T,IServiceCollection> action) where T : Type
        {
            foreach (var item in serv)
            {
                action(item);
            }
            return serv;
        }
    }


    public interface IOtokatariService { }
    public interface IOtokatariDbOperator { }
}
