using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Siren.Infrastructure.AssemblyLoad
{
    public static class AssemblyLoader
    {
        public static bool IsASnapshotAssembly(this Assembly assembly)
        {
            return 
                assembly
                    .GetTypes()
                    .Any(
                        o => o.BaseType == typeof(ModelSnapshot)
                    );
        }
    }
}