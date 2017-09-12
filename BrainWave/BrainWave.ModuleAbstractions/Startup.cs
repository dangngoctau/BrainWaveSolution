using BrainWave.Modules.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BrainWave.Module.Abstractions
{
    public abstract class StartupBase : IStartup
    {
        public virtual int Order => 0;

        public virtual void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
