using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HomeApplianceControl.Common;
using HomeApplianceControl.Common.CallerHelpers;
using HomeApplianceControl.Common.Settings;
using HomeApplianceControl.Domain.DAL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HomeApplianceControl
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

            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeApplianceControl", Version = "v1" });
            });

            var lgSettingsSection = Configuration.GetSection("LgSettings");
            services.Configure<LgSettings>(lgSettingsSection);

            var electroluxSettingsSection = Configuration.GetSection("ElectroluxSettings");
            services.Configure<ElectroluxSettings>(electroluxSettingsSection);

            services.AddDbContext<HomeApplianceControlContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<ILgCallerHelper, LgCallerHelper>();
            services.AddSingleton<IElectroluxCallerHelper, ElectroluxCallerHelper>();
            services.AddSingleton<CallerHelperManager>(s => 
                new CallerHelperManager(s.GetRequiredService<ILgCallerHelper>(),
                    s.GetRequiredService<IElectroluxCallerHelper>()));
            //InitLogManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeApplianceControl v1");
                });
            }

            HomeApplianceControlLogManager.LoggerFactory = loggerFactory;

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitLogManager()
        {
            var path = $"{System.IO.Path.GetFullPath(@"..\..\..")}/Configs/log4net.config";
            HomeApplianceControlLogManager.Init(path);
        }
    }
}
