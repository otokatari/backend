using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OtokatariBackend.Model.Response;

namespace OtokatariBackend.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        private readonly Dictionary<Type,int> _errors;
        public ErrorHandlerMiddleware(RequestDelegate next,ILogger<ErrorHandlerMiddleware> logger,ErrorHandlerOptions errors)
        {
            _next = next;
            _logger = logger;
            _errors = errors.ErrorCodes;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception ex)
            {
                await HandleGlobalExceptionAsync(context, ex);
            }
        }
        public Task HandleGlobalExceptionAsync(HttpContext context,Exception ex)
        {
            var response = new CommonResponse();

            if(_errors.ContainsKey(ex.GetType()))
            {
                response.StatusCode = _errors[ex.GetType()];
            }
            else response.StatusCode = -1100;
            _logger.LogError($"{DateTime.Now.ToString("yyyy/M/d H:mm:ss")} Global error occurred: {response.StatusCode}, {ex.Message}");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        public static OptionsBuilder Builder() => new OptionsBuilder();
        public class OptionsBuilder
        {
            private readonly Dictionary<Type,int> statusCodes = new Dictionary<Type, int>();

            public OptionsBuilder AddErrorStatusCode<TException>(int StatusCode) where TException : Exception
            {
                statusCodes.Add(typeof(TException),StatusCode);
                return this;
            }
            public IServiceCollection ConfigureErrorHandler(IServiceCollection services)
            {
                return services.AddSingleton(new ErrorHandlerOptions() { ErrorCodes = statusCodes });
            }
        }
    }
}