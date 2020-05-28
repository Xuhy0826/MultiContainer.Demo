using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Core;
using Order.Aggregate;
using Order.Model.OrderQuery;

namespace Order
{
    public interface IOrderRepository : : IRepository<Order, long>
    {
        Task<bool> Exist(long id);
        Task<bool> Exist(OrderQuery parameters);
        Task<IList<Order>> GetOrderCollectionAsync(OrderQuery parameters, bool byPage = false);
    }
}
