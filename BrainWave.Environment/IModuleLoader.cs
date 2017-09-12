using System.Collections.Generic;

namespace BrainWave.Environment
{
    public interface IModuleLoader
    {
        IEnumerable<ModuleEntry> LoadModules();
    }
}
