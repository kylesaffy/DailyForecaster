using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using DailyForecaster.Models;
using DailyForecaster.Data;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Azure.Identity;

namespace DailyForecaster
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateWebHostBuilder(args).Build();
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var context = services.GetRequiredService<FinPlannerContext>();
					DbInitializer.Initialize(context);
				}
				catch(Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occured while seeding the database");
				}
			}
			host.Run();
		}

		public static IHostBuilder CreateWebHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			//.ConfigureAppConfiguration((context, config) =>
			//{
			//	var keyVaultEndpoint = new Uri("https://dailyforecastervault.vault.azure.net/");
			//	config.AddAzureKeyVault(
			//	keyVaultEndpoint,
			//	new DefaultAzureCredential());
			//})
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			});
	}
}
