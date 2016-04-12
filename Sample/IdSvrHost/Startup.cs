using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdSvrHost.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using IdentityServer4.Core.Services.MongoDB;
using MongoDB.Driver;
using IdSvrHost.UI.Login;
using System.Diagnostics;

namespace IdSvrHost2
{
    public class Startup
    {
        private readonly IApplicationEnvironment _environment;

        public Startup(IApplicationEnvironment environment)
        {
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var _client = new MongoClient();

            services.AddInstance<IMongoDatabase>(_client.GetDatabase("IdentityServer"));

            var cert = new X509Certificate2(Path.Combine(_environment.ApplicationBasePath, "idsrv4test.pfx"), "idsrv3test");

            services.AddMongoDBTransientStores();

            var builder = services.AddIdentityServer(options =>
            {
                options.SigningCertificate = cert;
            });

            builder.AddMongoDBUsers();
            builder.AddMongoDBScopes();
            builder.AddMongoDBClients();

            builder.AddCustomGrantValidator<CustomGrantValidator>();


            // for the UI
            services
                .AddMvc()
                .AddRazorOptions(razor =>
                {
                    razor.ViewLocationExpanders.Add(new IdSvrHost.UI.CustomViewLocationExpander());
                });

            services.AddTransient<ILoginService, MongoDBLoginService>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Verbose);
            loggerFactory.AddDebug(LogLevel.Verbose);

            var sourceEventSwitch = new SourceSwitch("LoggingSample");
            sourceEventSwitch.Level = SourceLevels.Critical;
            
            loggerFactory.AddTraceSource(sourceEventSwitch,
                new EventLogTraceListener("Application"));

            var sourceFileSwitch = new SourceSwitch("LoggingSample");
            sourceFileSwitch.Level = SourceLevels.All;

            loggerFactory.AddTraceSource(sourceFileSwitch,
                new TextWriterTraceListener(@"\log\IdSvrHost.log")
            );

            app.UseDeveloperExceptionPage();
            app.UseIISPlatformHandler();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
