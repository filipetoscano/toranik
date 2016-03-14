using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ohm;

namespace Toranik.Endpoint
{
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env">Hosting environment.</param>
        public Startup( IHostingEnvironment env )
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile( "appsettings.json" )
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }


        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfigurationRoot Configuration
        {
            get;
            private set;
        }


        /// <summary>
        /// Adds custom services, which can then be used for DI.
        /// </summary>
        /// <param name="services">Service collection container.</param>
        public void ConfigureServices( IServiceCollection services )
        {
            // Add framework services.
            services.AddMvc();
        }


        /// <summary>
        /// Configure the HTTP pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Hosting environment.</param>
        /// <param name="loggerFactory">Loggers.</param>
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            loggerFactory.AddConsole( Configuration.GetSection( "Logging" ) );
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseWadl();
            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main( string[] args )
        {
            WebApplication.Run<Startup>( args );
        }
    }
}
