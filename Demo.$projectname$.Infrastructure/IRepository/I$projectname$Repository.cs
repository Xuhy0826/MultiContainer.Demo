using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Core;
using Demo.$projectname$.Domain.Aggregate;
using Demo.$projectname$.Domain.Model.$projectname$Query;

namespace Demo.$projectname$.Infrastructure
{
    public interface I$projectname$Repository : : IRepository<$projectname$, long>
    {
        Task<bool> Exist(long id);
        Task<bool> Exist($projectname$Query parameters);
        Task<IList<$projectname$>> Get$projectname$CollectionAsync($projectname$Query parameters, bool byPage = false);
    }
}
