using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceCommunicationLogging.ServiceProxies;
using ServiceCommunicationLogging.Middleware;
using System.Diagnostics;
using System.IO;
using ServiceCommunicationLogging.Middleware.DisplayLogsMiddleware;
using LogsDataAccess.Interfaces;
using LogsDataAccess.AzureImplementation;

namespace ServiceCommunicationLogging
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache

            services.AddSession();

            services.Configure<ServiceEndpoints>(Configuration.GetSection("ServiceEndpoints"));

            services.AddSingleton<ILogPersister, AzureLogPersister>();

            ////services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Trace);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();

            app.UseMiddleware<LoggingMiddleware>("ServiceCommunicationLogging NetCore App");

            app.UseStaticFiles();

            //app.UseWelcomePage(new WelcomePageOptions() { Path = "/welcome" });

            app.UseDisplayLogsPage(new DisplayLogsOptions() { Path = "/logs" });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
