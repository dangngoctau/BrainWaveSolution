using BrainWave.Environment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.FileProviders
{
    // todo: produce an extenstionManager to load modules. then add to singleton service.
    public static class ModularStaticFilesBuilder
    {
        public static IApplicationBuilder UseStaticFilesModules(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            // todo: reuse module loader.
            var modules = env.ContentRootFileProvider.GetDirectoryContents("Packages")
                .Where(c => c.IsDirectory/* && File.Exists(Path.Combine(c.PhysicalPath, "Module.txt"))*/);

            foreach (var module in modules)
            {
                var contentPath = Path.Combine(module.PhysicalPath, "Content");
                var contentSubPath = Path.Combine(module.Name, "Content");

                if (Directory.Exists(contentPath))
                {
                    IFileProvider fileProvider;
                    if (env.IsDevelopment())
                    {
                        fileProvider = new CompositeFileProvider(
                            //todo: map for development env.
                            //new ModuleProjectContentFileProvider(env.ContentRootPath, contentSubPath),
                            new PhysicalFileProvider(contentPath));
                    }
                    else
                    {
                        fileProvider = new PhysicalFileProvider(contentPath);
                    }

                    app.UseStaticFiles(new StaticFileOptions
                    {
                        RequestPath = "/" + module.Name,
                        FileProvider = fileProvider
                    });
                }
            }

            return app;
        }
    }
}
