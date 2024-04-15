using System.Collections.Generic;

namespace Siren.Infrastructure.AssemblyLoad.Configuration
{
    public interface IBuildConfigurationProvider
    {
        public IEnumerable<BuildConfiguration> Get();
    }
}