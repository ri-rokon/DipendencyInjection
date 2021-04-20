using DependencyInjection.Data;
using DependencyInjection.Middleware;
using DependencyInjection.Models;
using DependencyInjection.Service;
using DependencyInjection.Utility.AppSettingsClasses;
using DependencyInjection.Utility.DI_Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeCredit.Service.LifeTimeExample;

namespace DependencyInjection
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllersWithViews();
            //appsettings 
            services.AddAppSettingsConfig(Configuration);

            //services.AddScoped<IValidationChecker, AddressValidationChecker>(); for test  *1
            //services.AddScoped<IValidationChecker, CreditValidationChecker>(); for test *1
            //  services.AddScoped<IValidationChecker, CreditValidationChecker>(); if add multiple time its duplicated
            //*1
            //services.TryAddEnumerable(ServiceDescriptor.Scoped<IValidationChecker, AddressValidationChecker>());
            //services.TryAddEnumerable(ServiceDescriptor.Scoped<IValidationChecker, AddressValidationChecker>());

            services.TryAddEnumerable(new[] { ServiceDescriptor.Scoped<IValidationChecker, AddressValidationChecker>(),
                                           ServiceDescriptor.Scoped<IValidationChecker, CreditValidationChecker>() });

            services.AddScoped<ICreditValidator, CreditValidator>();






            services.AddTransient<IMarketForecaster, MarketForecaster>();
            services.TryAddTransient<IMarketForecaster, MarketForecasterV2>();
            services.Replace(ServiceDescriptor.Transient<IMarketForecaster, MarketForecaster>());
            services.RemoveAll<IMarketForecaster>();
            services.TryAddTransient<IMarketForecaster, MarketForecaster>();
            services.AddTransient<TransientService>();
            services.AddScoped<ScopedService>();
            services.AddSingleton<SingletonService>();

            //conditional implementation

            services.AddScoped<CreditApprovedHigh>();

            services.AddScoped<CreditApprovedLow>();

            services.AddScoped<Func<CreditApprovedEnum, ICreditApproved>>(ServiceProvider=> Range =>
            {
                switch (Range)
                {
                    case CreditApprovedEnum.Low: return ServiceProvider.GetService<CreditApprovedLow>();
                    case CreditApprovedEnum.High:return ServiceProvider.GetService<CreditApprovedHigh>();
                    default:return ServiceProvider.GetService<CreditApprovedLow>();
                }


            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            loggerFactory.AddFile("logs/creditApp-log-{Date}.txt");
            app.UseAuthorization();
            app.UseMiddleware<CustomMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
