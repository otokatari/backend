using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using OtokatariBackend.Model.DependencyInjection.Databases;
using OtokatariBackend.Model.DependencyInjection.RSAKey;
using OtokatariBackend.Model.DependencyInjection.Token;
using OtokatariBackend.Persistence.MongoDB.DependencyInjection;
using OtokatariBackend.Persistence.MySQL.DAO;
using OtokatariBackend.Services;
using OtokatariBackend.Services.Token;
using OtokatariBackend.Services.Users.UIDWorker;
using OtokatariBackend.Utils;

namespace OtokatariBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            // Configure HttpContextAccessor that can get http request & response context in filter.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            // Configure Jwt token generator information
            services.Configure<JwtTokenConfig>(Configuration.GetSection("JwtSignatureInfo"));
            services.AddSingleton<JwtManager>();
            services.AddSingleton<TokenManager>();


            // Configure databases connection.
            services.Configure<MongoClientConfiguration>(Configuration.GetSection("Mongo"));
            services.AddMongoDB();
            services.AddDbContext<OtokatariContext>(cfg => cfg.UseMySQL(Configuration["MySQL:ConnectionString"]));

            // Configure RSA utils
            services.Configure<RSAKeyFiles>(Configuration.GetSection("RsaKeys"));
            services.AddRsaUtils();

            // Configure services in the project.

            services.AddSingleton<IdWorker>();
            services.AddAllServices<IOtokatariService>();
            services.AddAllServices<IOtokatariDbOperator>();

            // Configure access controller.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//配置JWT服务
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuer = true,
                       ValidateIssuerSigningKey = true,
                       ValidAudience = Configuration["JwtSignatureInfo:Audience"],
                       ValidIssuer = Configuration["JwtSignatureInfo:Issuer"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSignatureInfo:SecurityKey"]))//拿到SecurityKey
                   };
                   options.Events = new JwtBearerEvents
                   {
                       OnAuthenticationFailed = context =>
                       {
                           //Give client some tips in header when request token is expired.
                           if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                           {
                               context.Response.Headers.Add("Token-Expired", "true");
                           }
                           return Task.CompletedTask;
                       }
                   };
               });
            // Configure redis cache to store revoked tokens.
            services.AddDistributedRedisCache(r =>
            {
                r.Configuration = Configuration["Redis:ConnectionString"];
                r.InstanceName = "token";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. 
        // You can also specify middleware orders here.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
