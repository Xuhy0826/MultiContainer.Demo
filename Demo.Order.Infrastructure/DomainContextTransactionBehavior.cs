using DotNetCore.CAP;
using Infrastructure.Core.Behaviors;

namespace Order
{
    public class DomainContextTransactionBehavior<TRequest, TResponse> : TransactionBehavior<DomainContext, TRequest, TResponse>
    {
        public DomainContextTransactionBehavior(DomainContext dbContext, ICapPublisher capBus)
            : base(dbContext, capBus)
        {
        }
    }
}
