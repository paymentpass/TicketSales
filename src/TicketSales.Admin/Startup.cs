using System;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicketSales.Admin.Consumers;
using TicketSales.Admin.Services;

namespace TicketSales.Admin
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton<TestMessageStore>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<TestEventHandler>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg => 
                {
                    cfg.Host(new Uri(Configuration["MassTransit:RabbitMqUri"]), hostConfigurator =>
                    {
                        hostConfigurator.Username(Configuration["MassTransit:RabbitMqUser"]);
                        hostConfigurator.Password(Configuration["MassTransit:RabbitMqPassword"]);
                    });

                    cfg.ReceiveEndpoint("admin", e =>
                    {
                        e.PrefetchCount = 16;

                        e.ConfigureConsumer<TestEventHandler>(provider);
                    });
                }));

                EndpointConvention.Map<Messages.Commands.TestCommand>(new Uri($"{Configuration["MassTransit:RabbitMqUri"]}/tickets"));
            });

            services.AddMassTransitHostedService();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
