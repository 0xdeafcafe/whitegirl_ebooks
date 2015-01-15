using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using System;
using WhiteGirlEbooks.DocumentDb;
using WhiteGirlEbooks.Models.Repositories;

namespace WhiteGirlEbooks
{
	public class Startup
	{
		/// <summary>
		/// The <see cref="IConfiguration"/> that stores application variables from <see cref="config.json"/> as well as Enviroment Variables.
		/// </summary>
		public static IConfiguration Configuration { get; set; }

		/// <summary>
		/// Settings to run on application startup.
		/// </summary>
		/// <param name="env">The <see cref="IHostingEnvironment"/> the application is running on.</param>
		public Startup(IHostingEnvironment env)
		{
			// Setup configuration sources.
			Configuration = new Configuration()
				.AddJsonFile("config.json")
				.AddEnvironmentVariables();
		}

		/// <summary>
		/// Configure the application.
		/// </summary>
		/// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
		public void Configure(IApplicationBuilder app, ILoggerFactory loggerfactory)
		{
			// log pls
			loggerfactory.AddConsole();

			// Enable the MVC framework
			app.UseMvc(routes =>
			{
				routes.MapRoute("Default", "{controller}/{action}", new { controller = "Home", action = "Index" });
			});

			// Enable Default Welcome Page
			app.UseWelcomePage();
		}

		/// <summary>
		/// Configure services to the application.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> created by ASP to regsiter services too.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			// Add the MVC Framework
			services.AddMvc();

			// Register the DocumentDb Context
			services.AddInstance<DdbDatabaseContext>(new DdbDatabaseContext(new DdbDatabaseOptions
			{
				DatabaseId = Configuration.Get("Data:DdbDatabaseId"),
				DatabaseAccessKey = Configuration.Get("Data:DdbDatabaseAccessKey"),
				DatabaseEndpointUri = new Uri(Configuration.Get("Data:DdbDatabaseEndpointUri"))
			}));

			// Register services that use the DocumentDb Context
			services.AddScoped<ITweetRepository, TweetRepository>();
			services.AddScoped<IMetadataRepository, MetadataRepository>();

			// Add the Web Api Framework 
			services.AddWebApiConventions();
		}
	}
}
