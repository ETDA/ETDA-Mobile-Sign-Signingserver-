using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SigningServer_TedaSign.Db;
using SigningServer_TedaSign.Models;
using SigningServer_TedaSign.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign
{
    public class Startup
    {
        private static readonly string APPSETTING_PATH = "/var/config/signing/appsettings.json";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Configuration = new ConfigurationBuilder()
                .AddJsonFile(APPSETTING_PATH)
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //DB Setting
            services.Configure<ConnectionStrings>(
                Configuration.GetSection(nameof(ConnectionStrings)));

            services.AddSingleton<IDatabaseSettings>(provider =>
                provider.GetRequiredService<IOptions<ConnectionStrings>>().Value);

            //Callback Setting
            services.Configure<CallbackSettings>(
                Configuration.GetSection(nameof(CallbackSettings)));

            services.AddSingleton<ICallbackSettings>(provider =>
                provider.GetRequiredService<IOptions<CallbackSettings>>().Value);

            //Config Setting
            services.Configure<ConfigSettings>(
                Configuration.GetSection(nameof(ConfigSettings)));

            services.AddSingleton<IConfigSettings>(provider =>
                provider.GetRequiredService<IOptions<ConfigSettings>>().Value);

            services.AddScoped<ExternalClientService>();

            services.AddScoped<SigningRequestService>();

            services.AddHttpClient();

            services.AddControllers();

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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
