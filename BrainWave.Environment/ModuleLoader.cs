using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrainWave.Environment
{
    public class ModuleLoader : IModuleLoader
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ModularExpanderOptions _modularExpanderOptions;

        public ModuleLoader(IHostingEnvironment hostingEnvironment, IOptions<ModularExpanderOptions> modularExpanderOptionsAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _modularExpanderOptions = modularExpanderOptionsAccessor.Value;
        }

        public IEnumerable<ModuleEntry> LoadModules()
        {
            // todo: check having module.txt
            var modules = _hostingEnvironment.ContentRootFileProvider.GetDirectoryContents(_modularExpanderOptions.Packages)
            .Where(c => c.IsDirectory/* && File.Exists(Path.Combine(c.PhysicalPath, "Module.txt"))*/);
            var moduleEntries = new List<ModuleEntry>();
            foreach (var module in modules)
            {
                var assembly = Assembly.Load(new AssemblyName(module.Name));
                if (assembly == null)
                {
                    continue;
                }

                var exportedStartupTypes = assembly.ExportedTypes
                    .Where(IsComponentType)
                    .Where(t => typeof(Modules.Abstractions.IStartup).IsAssignableFrom(t));
                moduleEntries.Add(new ModuleEntry
                {
                    Id = module.Name,
                    PhysicalPath = module.PhysicalPath,
                    ExportedStartupTypes = exportedStartupTypes
                });
            }

            return moduleEntries;
        }

        private static bool IsComponentType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass && !typeInfo.IsAbstract && typeInfo.IsPublic;
        }
    }
}
