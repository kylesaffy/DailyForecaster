using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DailyForecaster.Models;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Net;

namespace DailyForecaster
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
			services.AddDbContext<FinPlannerContext>(opt =>
			opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
			services.AddControllers();//.AddNewtonsoftJson(x => {
				//x.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
				//x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
				//});
			services.AddCors(options =>
			{
				options.AddPolicy("AllowOrigin",
						 //builder => builder.WithOrigins("https://localhost:44307", "https://12dceb811896.ngrok.io")
						 builder=>builder.AllowAnyOrigin()
						 .AllowAnyHeader()
						 .AllowAnyMethod());
			});
			services.Configure<TwilioAccountDetails>(Configuration.GetSection("TwilioAccountDetails"));
			// get key for SAS
			string value = Configuration["Token"];
			// get file
			//var webRequest = WebRequest.Create(@"https://storageaccountmoney9367.blob.core.windows.net/emailimages/moneyminders-firebase-adminsdk-4q78b-f0036fedba.json");
			//using (var response = webRequest.GetResponse())
			//using (var content = response.GetResponseStream())
			//{											  
			var pathKey = Path.Combine(Directory.GetCurrentDirectory(), "moneyminders-firebase-adminsdk-4q78b-85e3b4b600.json");
				FirebaseApp.Create(new AppOptions
				{
					Credential = GoogleCredential.FromFile(pathKey)
				});
			//}
			//*services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors("AllowOrigin");

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
			
		}
	}
}
