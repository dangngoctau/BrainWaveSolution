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
            var serviceProvider = services.BuildServiceProvider();
            var env = serviceProvider.GetRequiredService<IHostingEnvironment>();
            var modules = env.ContentRootFileProvider.GetDirectoryContents("Packages")
                .Where(c => c.IsDirectory/* && File.Exists(Path.Combine(c.PhysicalPath, "Module.txt"))*/);
            foreach (var module in modules)
            {
                var assembly = Assembly.Load(new AssemblyName(module.Name));
                // Get export types.
                if (assembly == null)
                {
                    // 
                }
                else
                {
                    var exportedTypes = assembly.ExportedTypes.Where(IsComponentType);
                    exportedTypes = exportedTypes.Where(t => typeof(IStartup).IsAssignableFrom(t));

                    foreach (Type startup in exportedTypes)
                    {
                        services.AddSingleton(typeof(IStartup), startup);
                    }
                }
            }

            return services;
        }

        public static IMvcBuilder AddMvcModules(this IServiceCollection services)
        {
            var builder = services.AddMvc()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationExpanders.Add(new ModularViewLocationExpanderProvider());
                    options.AddLayoutLocationFormats();
                });

            return builder;
        }

        public static RazorViewEngineOptions AddLayoutLocationFormats(this RazorViewEngineOptions options)
        {
            // todo: move Packages, DefaultTheme to config.
            options.AreaViewLocationFormats.Clear();
            options.AreaViewLocationFormats.Add("/Packages/DefaultTheme/{2}/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Packages/DefaultTheme/Views/" + "{0}" + RazorViewEngine.ViewExtension);

            return options;
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
                app.UseExceptionHandler("/Home/Error");
            }

            // todo: move router builder to Builders.
            var routes = new RouteBuilder(app)
            {
                DefaultHandler = serviceProvider.GetRequiredService<MvcRouteHandler>(),
            };

            var inlineConstraintResolver = app.ApplicationServices.GetRequiredService<IInlineConstraintResolver>();
            routes.Routes.Add(new Route(
                routes.DefaultHandler,
                "areaRoute",
                "{area:exists}/{controller}/{action}/{id?}",
                null,
                null,
                null,
                inlineConstraintResolver)
            );

            var startups = serviceProvider.GetServices<IStartup>();
            foreach (var startup in startups)
            {
                startup.Configure(app, routes, serviceProvider);
            }

            app.UseStaticFilesModules();
            app.UseRouter(routes.Build());
        }

        private static bool IsComponentType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass && !typeInfo.IsAbstract && typeInfo.IsPublic;
        }
    }

    public class ModularViewLocationExpanderProvider : IViewLocationExpander
    {
        // todo: move "Packages" to service options.
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var result = new List<string>();
            result.AddRange(viewLocations);
            var extensionViewsPath = "/Packages/" + context.AreaName + "/Views";
            result.Add(extensionViewsPath + "/{1}/{0}" + RazorViewEngine.ViewExtension);
            result.Add(extensionViewsPath + "/Shared/{0}" + RazorViewEngine.ViewExtension);
            return result;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
