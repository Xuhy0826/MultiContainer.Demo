using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.User.Domain.Model;
using Infrastructure.Core;

namespace Demo.User.Infrastructure
{
    public interface IUserRepository : IRepository<Domain.Aggregate.User, long>
    {
        Task<bool> Exist(long id);
        Task<bool> Exist(UserQuery parameters);
        Task<IList<Domain.Aggregate.User>> GetUserCollectionAsync(UserQuery parameters, bool byPage = false);
    }
}
