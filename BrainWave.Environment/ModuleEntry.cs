using System;
using System.Collections.Generic;
using System.Reflection;

namespace BrainWave.Environment
{
    public class ModuleEntry
    {
        public string Id { get; set; }
        public string PhysicalPath { get; set; }
        public IEnumerable<Type> ExportedStartupTypes { get; set; }
    }
}
