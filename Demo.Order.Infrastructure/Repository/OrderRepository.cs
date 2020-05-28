using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Mark.Common.Services;
using Order.Aggregate;
using Order.Model;
using Order.Container;

namespace Order
{
    public class OrderRepository : Repository<Order, long, DomainContext>, IOrderRepository
    {
        private readonly IPropertyMappingService _propertyMappingService;
        public OrderRepository(DomainContext context, IPropertyMappingService propertyMappingService) : base(context)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException();
        }

        public async Task<bool> Exist(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Exist(OrderQuery parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Order>> GetOrderCollectionAsync(OrderQuery parameters, bool byPage = false)
        {
            throw new NotImplementedException();
        }
    }
}
