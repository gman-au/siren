using System.Collections.Generic;
using System.Threading.Tasks;
using SchemaSearch.Domain.Schema;

namespace Siren.Infrastructure.SchemaSearch
{
    public interface ISearchApplication
    {
        Task<IList<SchemaTable>> PerformAsync(string connectionString);
    }
}