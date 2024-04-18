using System.Collections.Generic;

namespace Siren.Infrastructure.AssemblyLoad.Configuration
{
    public class BuildConfigurationProvider : IBuildConfigurationProvider
    {
        public IEnumerable<BuildConfiguration> Get()
        {
            var buildConfigurations = new List<BuildConfiguration>
            {
                new()
                {
                    StepsBackToEntityName = 3,
                    StepsForwardToPropertyBuilder = 3
                }
            };

            return buildConfigurations;
        }
    }
}