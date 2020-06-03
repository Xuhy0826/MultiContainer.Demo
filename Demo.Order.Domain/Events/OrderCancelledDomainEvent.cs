using Domain.Abstractions;

namespace Demo.Order.Domain.Events
{
    public class OrderCancelledDomainEvent : IDomainEvent
    {
        public global::Order.Aggregate.OrderAggregate.Order Order { get; }

        public OrderCancelledDomainEvent(global::Order.Aggregate.OrderAggregate.Order order)
        {
            Order = order;
        }
    }
}
