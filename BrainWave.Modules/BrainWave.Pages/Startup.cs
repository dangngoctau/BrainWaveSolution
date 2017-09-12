using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using BrainWave.Module.Abstractions;

namespace BrainWave.Pages
{
    public class Startup : StartupBase
    {
        public override void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaRoute(
               name: "BrainWave.Pages.Default",
               areaName: "BrainWave.Pages",
               template: "",
               defaults: new { controller = "Page", action = "Index" }
           );
            routes.MapAreaRoute(
                name: "BrainWave.Pages.Generic",
                areaName: "BrainWave.Pages",
                template: "BrainWave.Pages/{controller}/{action}/{id?}",
                defaults: new { controller = "Page", action = "Index" }
);
        }
    }
}
