using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Routing
{
    public static class ModularRouteBuilder
    {
        public static IRouteBuilder CreateModularRouteBuilder(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
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

            return routes;
        }
    }
}
