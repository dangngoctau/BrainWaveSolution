using BrainWave.Environment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IStartup = BrainWave.Modules.Abstractions.IStartup;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBrainWaveCms(this IServiceCollection services, IConfiguration configuration)
        {
            // todo: move packages name to options
            var builder = services.AddMvcModules();
            services.AddTransient<IModuleLoader, ModuleLoader>();
            services.Configure<ModularExpanderOptions>(c => c.Packages = "Packages");
            var serviceProvider = services.BuildServiceProvider();
            var env = serviceProvider.GetRequiredService<IHostingEnvironment>();
            var moduleLoader = serviceProvider.GetRequiredService<IModuleLoader>();
            var modules = moduleLoader.LoadModules();
            foreach (var module in modules)
            {
                foreach (Type startup in module.ExportedStartupTypes)
                {
                    services.AddSingleton(typeof(IStartup), startup);
                }
            }

            return services;
        }

        public static IMvcBuilder AddMvcModules(this IServiceCollection services)
        {
            var builder = services.AddMvc()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationExpanders.Add(new ModularViewLocationExpandBuilder());
                    options.AddModularLayoutLocationFormats();
                });

            return builder;
        }

        public static void ConfigureBrainWaveCms(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var env = serviceProvider.GetRequiredService<IHostingEnvironment>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                // todo: Declare this page in module pages.
                app.UseExceptionHandler("/Home/Error");
            }

            var routeBuilder = app.CreateModularRouteBuilder();
            var startups = serviceProvider.GetServices<IStartup>();
            foreach (var startup in startups)
            {
                startup.Configure(app, routeBuilder, serviceProvider);
            }

            app.UseStaticFilesModules();
            app.UseRouter(routeBuilder.Build());
        }

        private static bool IsComponentType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass && !typeInfo.IsAbstract && typeInfo.IsPublic;
        }
    }
}
